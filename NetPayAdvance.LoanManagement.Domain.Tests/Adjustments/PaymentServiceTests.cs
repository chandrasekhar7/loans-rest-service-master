using System;
using System.Collections.Generic;
using System.Linq;
using NetPayAdvance.LoanManagement.Domain.Entity;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using Xunit;

namespace NetPayAdvance.LoanManagement.Domain.Tests.Adjustments;

public class PaymentServiceTests
{
    private readonly List<Statement> defaultStatements = new()
    {
        new Statement(new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(16)),
            new Amount(100, 10, 50, 0, 0),
            new Amount(100, 10, 50, 0, 0),
            new Period(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now).AddDays(16)),
            DateOnly.FromDateTime(DateTime.Now).AddDays(30)),
        new Statement(new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(30)),
            new Amount(50, 0, 0, 0, 0),
            new Amount(50, 0, 0, 0, 0),
            new Period(DateOnly.FromDateTime(DateTime.Now.AddDays(17)), DateOnly.FromDateTime(DateTime.Now).AddDays(30)),
            DateOnly.FromDateTime(DateTime.Now).AddDays(30)),
        new Statement(new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(45)),
            new Amount(30, 0, 0, 0, 0),
            new Amount(30, 0, 0, 0, 0),
            new Period(DateOnly.FromDateTime(DateTime.Now.AddDays(31)), DateOnly.FromDateTime(DateTime.Now).AddDays(45)),
            DateOnly.FromDateTime(DateTime.Now).AddDays(45))
    };
    
    [Fact]
    public void ThrowForInvalidAmount()
    {
        var s = new PaymentService();
        Assert.Throws<DomainLayerException>(() => s.Payment(1, Amount.Zero.AddPrincipal(100), new List<Statement>(), -40, "NMR", 1));
    }

    [Fact]
    public void ApplyZeroPayment()
    {
        var s = new PaymentService();
        var amount = s.Payment(1, Amount.Zero.AddPrincipal(100), new List<Statement>(), 0, "NMR", 1);
        Assert.Equal(0, -amount.Adjustment.Amount.Principal);
    }
    
    [Fact]
    public void ApplyPartialPayment()
    {
        var s = new PaymentService();
        var amount = s.Payment(1, Amount.Zero.AddPrincipal(100), new List<Statement>(), 40, "NMR", 1);
        Assert.Equal(40, -amount.Adjustment.Amount.Principal);
    }
    
    [Fact]
    public void ApplyToCabFees()
    {
        var s = new PaymentService();
        var amount = s.Payment(1, Amount.Zero.AddPrincipal(100), defaultStatements, 50, "NMR", 1);
        Assert.Equal(50, -amount.Adjustment.Amount.CabFees);
    }
    
    [Fact]
    public void ApplyToLoanBalance()
    {
        var s = new PaymentService();
        var amount = s.Payment(1, Amount.Zero.AddPrincipal(100), defaultStatements, 70, "NMR", 1);
        Assert.Equal(50, -amount.Adjustment.Amount.CabFees);
        Assert.Equal(10, -amount.Adjustment.Amount.Interest);
        Assert.Equal(10, -amount.Adjustment.Amount.Principal);
    }
    
    [Fact]
    public void OverPayment()
    {
        var s = new PaymentService();
        var amount = s.Payment(1, new Amount(10,0,0,0,0), new List<Statement>(), 30, "NMR", 1);
        Assert.Equal(30, -amount.Adjustment.Amount.Principal);
    }
   
    [Fact]
    public void AppliesPrincipal()
    {
        var s = new PaymentService();
        var amount = s.Payment(1, Amount.Zero.AddPrincipal(10), new List<Statement>(), 10, "EC1", 1);
        Assert.Equal(10, -amount.Adjustment.Amount.Principal);
    }
    
    [Fact]
    public void AppliesInterest()
    {
        var s = new PaymentService();
        var amount = s.Payment(1, Amount.Zero.AddInterest(10), new List<Statement>(), 10, "EC1", 1);
        Assert.Equal(10, -amount.Adjustment.Amount.Interest);
    }
    
    [Fact]
    public void AppliesCabFees()
    {
        var s = new PaymentService();
        var amount = s.Payment(1, Amount.Zero.AddCabFees(10), new List<Statement>(), 10, "EC1", 1);
        Assert.Equal(10, -amount.Adjustment.Amount.CabFees);
    }
    
    [Fact]
    public void AppliesNsf()
    {
        var s = new PaymentService();
        var amount = s.Payment(1, Amount.Zero.AddNsf(10), new List<Statement>(), 10, "EC1", 1);
        Assert.Equal(10, -amount.Adjustment.Amount.Nsf);
    }
    
    [Fact]
    public void AppliesLateFees()
    {
        var s = new PaymentService();
        var amount = s.Payment(1, Amount.Zero.AddLateFees(10), new List<Statement>(), 10, "EC1", 1);
        Assert.Equal(10, -amount.Adjustment.Amount.LateFees);
    }

    [Fact]
    public void ApplyToStatement()
    {
        var s = new PaymentService();
        var loanBalance = new Amount(150, 10, 50, 0, 0);
        var adjustment = s.Payment(1, loanBalance, defaultStatements, 100, "NMR", 1);
        var expectedAdjustment = new LoanAdjustment(1, "NMR", new Adjustment(AdjustmentType.Payment, new Amount(-40,-10,-50,0,0)),
            new List<StatementAdjustment>()
            {
                new(AdjustmentType.Payment, new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(16)), new Amount(-40,-10,-50,0,0), "NMR")
            },1);
        Assert.Equal(expectedAdjustment.Adjustment.Amount,adjustment.Adjustment.Amount);
        Assert.Equal(expectedAdjustment.StatementAdjustments.First().Adjustment.Amount,adjustment.StatementAdjustments.First().Adjustment.Amount);
    }
    
    [Fact]
    public void ApplyToStatements()
    {
        var s = new PaymentService();
        var loanBalance = new Amount(150, 10, 50, 0, 0);
        var adjustment = s.Payment(1, loanBalance, defaultStatements, 180, "NMR", 1);
        var expectedAdjustment = new LoanAdjustment(1, "NMR", new Adjustment(AdjustmentType.Payment, new Amount(-120,-10,-50,0,0)),
            new List<StatementAdjustment>()
            {
                new(AdjustmentType.Payment, new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(16)), new Amount(-100,-10,-50,0,0), "NMR"),
                new(AdjustmentType.Payment, new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(45)), new Amount(-20,0,0,0,0), "NMR")
            },1);
        Assert.Equal(expectedAdjustment.Adjustment.Amount,adjustment.Adjustment.Amount);
        foreach (var a in expectedAdjustment.StatementAdjustments)
        {
            foreach (var ex in adjustment.StatementAdjustments.Where(x => x.StatementId == a.StatementId))
            {
                Assert.Equal(a.Adjustment.Amount, ex.Adjustment.Amount);
            }
        }
    }
    
    [Fact]
    public void ApplyToLastStatements()
    {
        var s = new PaymentService();
        var loanBalance = new Amount(150, 10, 50, 0, 0);
        var adjustment = s.Payment(1, loanBalance, defaultStatements, 180, "NMR", 1);
        var expectedAdjustment = new LoanAdjustment(1, "NMR", new Adjustment(AdjustmentType.Payment, new Amount(-120,-10,-50,0,0)),
            new List<StatementAdjustment>()
            {
                new(AdjustmentType.Payment, new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(16)), new Amount(-100,-10,-50,0,0), "NMR"),
                new(AdjustmentType.Payment, new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(45)), new Amount(-20,0,0,0,0), "NMR")
            },1);
        Assert.Equal(expectedAdjustment.StatementAdjustments.First().StatementId,adjustment.StatementAdjustments.First().StatementId);
        Assert.Equal(expectedAdjustment.StatementAdjustments.Last().StatementId,adjustment.StatementAdjustments.Last().StatementId);
    }
    
    [Fact]
    public void ApplyPaymentToCorrectStatements()
    {
        var s = new PaymentService();
        var loanBalance = new Amount(180, 10, 50, 0, 0);
        var adjustment = s.Payment(1, loanBalance, defaultStatements,230 , "NMR", 1);
        var expectedAdjustment = new LoanAdjustment(1, "NMR", new Adjustment(AdjustmentType.Payment, new Amount(-170,-10,-50,0,0)),
            new List<StatementAdjustment>()
            {
                new(AdjustmentType.Payment, new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(16)), new Amount(-100,-10,-50,0,0), "NMR"),
                new(AdjustmentType.Payment, new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(45)), new Amount(-30,0,0,0,0), "NMR"),
                new(AdjustmentType.Payment, new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(30)), new Amount(-40,0,0,0,0), "NMR")
            },1);
        foreach (var a in expectedAdjustment.StatementAdjustments)
        {
            foreach (var ex in adjustment.StatementAdjustments.Where(x => x.StatementId == a.StatementId))
            {
                Assert.Equal(a.Adjustment.Amount, ex.Adjustment.Amount);
            }
        }
    }
}