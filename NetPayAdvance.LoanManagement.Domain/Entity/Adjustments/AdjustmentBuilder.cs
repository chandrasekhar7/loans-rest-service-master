using System.Collections.Generic;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;

public class AdjustmentBuilder
{
    private readonly AdjustmentType adjustmentType;
    private Adjustment Adjustment { get; }
    private int? PaymentId { get; }
    private int LoanId { get; }
    private string Teller { get; }
    public Amount Amount => Adjustment.Amount;
    private List<StatementAdjustment> StatementAdjustments { get; } = new List<StatementAdjustment>();

    public AdjustmentBuilder(AdjustmentType adjustmentType, int loanId, string teller, int? paymentId = null)
    {
        Adjustment = new Adjustment(adjustmentType, Amount.Zero);
        this.adjustmentType = adjustmentType;
        LoanId = loanId;
        Teller = teller;
        PaymentId = paymentId;
    }

    public AdjustmentBuilder ApplyStatementAdjustment(Statement s, Amount a)
    {
        StatementAdjustments.Add(new StatementAdjustment(adjustmentType, s.StatementId, a, Teller));
        ApplyAdjustment(a);
        return this;
    }

    public AdjustmentBuilder ApplyAdjustment(Amount a)
    {
        Adjustment.Amount += a;
        return this;
    }

    public LoanAdjustment Build()
    {
        return new LoanAdjustment(LoanId, Teller, Adjustment, StatementAdjustments, PaymentId);
    }
}