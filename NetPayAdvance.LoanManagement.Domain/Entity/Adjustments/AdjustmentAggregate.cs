using System;
using System.Collections.Generic;
using System.Linq;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;

public class AdjustmentAggregate : DomainEntity
{
    private readonly List<Statement> statements = new();
    private readonly List<LoanAdjustment> adjustments = new();
    
    public int LoanId { get; }
    public Loan Loan { get; }
    public List<Statement> Statements => statements.ToList();
    public List<LoanAdjustment> Adjustments => adjustments.ToList();

    private AdjustmentAggregate() { }

    public AdjustmentAggregate(Loan loan, List<Statement>? statements = null, List<LoanAdjustment>? adjustments = null)
    {
        Loan = loan;
        LoanId = loan.LoanId;
        this.statements = statements;
        this.adjustments = adjustments ?? new List<LoanAdjustment>();
    }

    private void ClearBalanceIfLow(string teller)
    {
        if (Loan.Balance.Amount.Principal > 0 && Loan.Balance.Amount.Principal < 5)
        {
            var builder = new AdjustmentBuilder(AdjustmentType.WriteOff, Loan.LoanId, teller);
            foreach (var s in Statements.Where(x => x.Balance.Amount.Total > 0))
            {
                builder.ApplyStatementAdjustment(s, -s.Balance.Amount);
            }

            if (Loan.Balance.Amount + builder.Amount != Amount.Zero)
            {
                builder.ApplyAdjustment(-(Loan.Balance.Amount + builder.Amount));
            }

            var adjustment = builder.Build();
            Adjustments.Add(adjustment);
            Loan.AddAmount(adjustment.Adjustment.Amount);
        }
    }

    private void ApplyAdjustment(LoanAdjustment adjustment)
    {
        adjustments.Add(adjustment);
        foreach (var a in adjustment.StatementAdjustments)
        {
            statements.Find(s => s.StatementId == a.StatementId)?.ApplyAdjustment(a.Adjustment.Amount);
        }

        Loan.AddAmount(adjustment.Adjustment.Amount);
        ClearBalanceIfLow(adjustment.Teller);
    }

    public void ApplySkipPayment(DateOnly origDate, string teller)
    {
        var statement = Statements.FirstOrDefault(x => x.OrigDueDate == origDate) ?? throw new Exception("Invalid OrigDueDate");
        var list = new List<StatementAdjustment>();
        if (statement.Balance.Amount.Total == 0)
        {
            throw new Exception("Can't skip payment on a statement with a $0 balance");
        }
        if (statement.Balance.Amount.Interest + statement.Balance.Amount.CabFees > 0)
        {
            list.Add(new StatementAdjustment(AdjustmentType.Discount,statement.StatementId,
                            new Amount(0,-statement.Balance.Amount.Interest,-statement.Balance.Amount.CabFees,0,0),teller));
        }
        if (statement.Balance.Amount.Principal > 0)
        {
            list.Add(new StatementAdjustment(AdjustmentType.SkipPayment,statement.StatementId,
                             new Amount(-statement.Balance.Amount.Principal,0,0,0,0),teller));
        }
        
        var adjustment = new LoanAdjustment(Loan.LoanId, teller, new Adjustment(AdjustmentType.SkipPayment,
                            new Amount(0,-statement.Balance.Amount.Interest,-statement.Balance.Amount.CabFees,0,0)), list);
        ApplyAdjustment(adjustment);
    }

    public void ApplyRescind(int paymentId, decimal amount, string teller, int rescindPaymentId, StatementId? statementId = null)
    {
        if (amount < 0)
        {
            throw new InvalidOperationException("You must use positive values for 'amount'");
        }
        
        var stmtList = statements.Where(s => s.StatementId != statementId && s.Balance.Amount.Total > 0).ToList();
        if (statementId != null)
        {
            var stmt = statements.FirstOrDefault(s => s.StatementId == statementId) ??
                       throw new InvalidOperationException($"Cannot create rescind adjustment: Statement {statementId} does not exist for loan {Loan.LoanId}");
            stmtList.Insert(0, stmt);
        }

        //Date of the adjustment corresponding to the rescinded payment
        var adjDate = (adjustments.FirstOrDefault(a => a.PaymentID == rescindPaymentId) ?? 
            throw new InvalidOperationException($"Cannot create rescind adjustment: No adjustment exists for loan ID: {Loan.LoanId} with paymentID {rescindPaymentId}")).Adjustment.CreatedOn;
        //This is messier than I would like, but it calculates the interest specifically for the ammount if it's not fully paying off a non-loc
        if (amount == Loan.Balance.Amount.Principal && Loan.Config.LoanType != LoanType.LineOfCredit)
        {
            var la = new PaymentService().Rescind(Loan.LoanId, Loan.Balance.Amount, stmtList, Loan.Balance.Amount, teller, paymentId, Loan.Config.LoanType);
            ApplyAdjustment(la);

            Loan.Rescind();
        }
        else {
            //Calculates the interest if we aren't paying off an entire non LOC loan
            var interestDays = Convert.ToDecimal(DateTime.Now.Date.Subtract(adjDate.Date).TotalDays);
            var interestPerDay = ((amount * Loan.Config.Apr / 365));
            var adjustmentAmount = new Amount(amount, (Decimal.Truncate(100 * Convert.ToDecimal(interestPerDay * interestDays)) / 100), 0, 0, 0);


            var la = new PaymentService().Rescind(Loan.LoanId, Loan.Balance.Amount, stmtList, adjustmentAmount, teller, paymentId, Loan.Config.LoanType);
            ApplyAdjustment(la);
        }    
        
    }

    public void ApplyPayment(int paymentId, decimal amount, string teller, StatementId? statementId = null)
    {
        if (amount < 0)
        {
            throw new InvalidOperationException("You must use positive values for 'amount'");
        }

        var stmtList = statements.Where(s => s.StatementId != statementId && s.Balance.Amount.Total > 0).ToList();
        if (statementId != null)
        {
            var stmt = statements.FirstOrDefault(s => s.StatementId == statementId) ??
                       throw new InvalidOperationException("Statement does not exist");
            stmtList.Insert(0, stmt);
        }

        var la = new PaymentService().Payment(Loan.LoanId, Loan.Balance.Amount, stmtList, amount, teller, paymentId);
        ApplyAdjustment(la);
    }

    public void ApplyDisbursement(int paymentId, decimal amount, string teller)
    {
        if (amount < 0)
        {
            throw new InvalidOperationException("You must use positive values for 'amount'");
        }

        var la = new LoanAdjustment(Loan.LoanId, teller,
            new Adjustment(AdjustmentType.Disbursement, Amount.Zero.AddPrincipal(amount)),
            new List<StatementAdjustment>(),
            paymentId);
        ApplyAdjustment(la);
    }

    public void ApplyCredit(Credit type, Amount amount, string teller, StatementId? statementId = null)
    {
        if (amount.Total < 0)
        {
            throw new InvalidOperationException("Amount must be greater than zero");
        }

        var stmts = new List<StatementAdjustment>();
        if (statementId != null)
        {
            if (!statements.Any(s => s.StatementId == statementId))
            {
                throw new NullReferenceException($"Statement: {statementId} does not exist");
            }

            stmts.Add(new StatementAdjustment(type, statementId, new Amount(amount), teller));
        }

        var la = new LoanAdjustment(Loan.LoanId, teller,
            new Adjustment(type, amount), stmts);
        ApplyAdjustment(la);
    }

    public void ApplyDebit(Debit type, Amount amount, string teller, StatementId? statementId = null)
    {
        if (amount.Total > 0)
        {
            throw new InvalidOperationException("Amount must be less than zero");
        }

        var stmts = new List<StatementAdjustment>();
        if (statementId != null)
        {
            var stmt = statements.FirstOrDefault(s => s.StatementId == statementId) 
                       ?? throw new NullReferenceException($"Statement: {statementId} does not exist");
            if (IsNegativeAdjustment(stmt.Balance.Amount, -amount))
            {
                throw new InvalidOperationException("amount is greater than the statement balance");
            }

            stmts.Add(new StatementAdjustment(type, statementId, amount, teller));
        }

        var la = new LoanAdjustment(Loan.LoanId, teller, new Adjustment(type, amount), stmts);
        if (Statements.Any(s => IsNegativeAdjustment(Loan.Balance.Amount + la.Adjustment.Amount, s.Balance.Amount +
                (stmts.FirstOrDefault(a => a.StatementId == s.StatementId)?.Adjustment.Amount ?? Amount.Zero))))
        {
            throw new InvalidOperationException("This would leave a statement with a greater balance than the loan");
        }

        ApplyAdjustment(la);
    }

    private bool IsNegativeAdjustment(Amount a, Amount b) => a.Principal < b.Principal || a.Interest < b.Interest ||
                                                             a.CabFees < b.CabFees || a.Nsf < b.Nsf ||
                                                             a.LateFees < b.LateFees;
}