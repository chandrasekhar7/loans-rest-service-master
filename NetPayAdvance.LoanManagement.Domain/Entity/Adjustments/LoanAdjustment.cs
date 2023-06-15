using System;
using System.Collections.Generic;
using System.Linq;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;

public class LoanAdjustment : DomainEntity
{
    public int AdjustmentID { get; }

    public int LoanID { get; }

    public int? PaymentID { get; }

    public Adjustment Adjustment { get; }

    public string Teller { get; }

    public List<StatementAdjustment> StatementAdjustments { get; }

    private LoanAdjustment() {}

    public LoanAdjustment(int loanID, string teller, Adjustment adj, 
        List<StatementAdjustment> statementAdjustments, int? paymentId = null)
    {
        LoanID = loanID;
        Teller = teller;
        Adjustment = adj;
        StatementAdjustments = statementAdjustments;
        PaymentID = paymentId;
    }

    public LoanAdjustment Undo(AdjustmentType adjustmentType, string teller)
    {
        if ((Enum.IsDefined(typeof(Credit), (int)adjustmentType) &&
             Enum.IsDefined(typeof(Credit), (Adjustment.AdjustmentCodeID))) ||
            (Enum.IsDefined(typeof(Debit), (int)adjustmentType) &&
             Enum.IsDefined(typeof(Debit), (Adjustment.AdjustmentCodeID))))
        {
            throw new InvalidOperationException("You cannot apply credit to credit or debit to debit");
        }

        return new LoanAdjustment(LoanID, teller, new Adjustment(adjustmentType, -Adjustment.Amount),
            StatementAdjustments
                .Select(s => new StatementAdjustment(adjustmentType, s.StatementId, -s.Adjustment.Amount, teller))
                .ToList());
    }
}