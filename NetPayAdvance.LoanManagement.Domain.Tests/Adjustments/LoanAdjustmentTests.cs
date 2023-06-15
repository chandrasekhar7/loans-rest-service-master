using System;
using System.Collections.Generic;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using Xunit;

namespace NetPayAdvance.LoanManagement.Domain.Tests.Adjustments;

public class LoanAdjustmentTests
{
    [Fact]
    public void CanGetRevertedDebitAdjustment()
    {
        var adj = new LoanAdjustment(1, "ZZZ", new Adjustment(Debit.Payment, Amount.Zero.AddPrincipal(-50)),
            new List<StatementAdjustment>(), 1);

        var reversed = adj.Undo(AdjustmentType.ReturnPayment, "YYY");
        Assert.Equal(-adj.Adjustment.Amount, reversed.Adjustment.Amount);
        Assert.Equal((int)AdjustmentType.ReturnPayment, reversed.Adjustment.AdjustmentCodeID);
    }
    
    [Fact]
    public void CanGetRevertedCreditAdjustment()
    {
        var adj = new LoanAdjustment(1, "ZZZ", new Adjustment(Debit.CorrectionDecrease, Amount.Zero.AddPrincipal(-50)),
            new List<StatementAdjustment>(), 1);

        var reversed = adj.Undo(AdjustmentType.CorrectionIncrease, "YYY");
        Assert.Equal(-adj.Adjustment.Amount, reversed.Adjustment.Amount);
        Assert.Equal((int)AdjustmentType.CorrectionIncrease, reversed.Adjustment.AdjustmentCodeID);
    }
    
    [Fact]
    public void ThrowsWhenRevertingSameType()
    {
        var adj = new LoanAdjustment(1, "ZZZ", new Adjustment(AdjustmentType.CorrectionDecrease, Amount.Zero.AddPrincipal(-50)),
            new List<StatementAdjustment>(), 1);

        Assert.Throws<InvalidOperationException>(() => adj.Undo(AdjustmentType.CorrectionDecrease, "YYY"));
    }
}