using System;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using Xunit;

namespace NetPayAdvance.LoanManagement.Domain.Tests.Loans;

public class LoansTest
{
    [Fact]
    public void ThrowsWhenCancelLoanWithCertainStatus()
    {
        var loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.Open, new Amount(1, 0, 0, 0, 0));

        Assert.Throws<InvalidOperationException>(() => loan.Cancel());
        
        loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.Closed, new Amount(1, 0, 0, 0, 0));
        Assert.Throws<InvalidOperationException>(() => loan.Cancel());
        
        loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.Rescinded, new Amount(1, 0, 0, 0, 0));
        Assert.Throws<InvalidOperationException>(() => loan.Cancel());
    }
    
    [Fact]
    public void CanCancelNotStartedLoan()
    {
        var loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.NotStarted, new Amount(1, 0, 0, 0, 0));
        loan.Cancel();
        Assert.Equal(LoanStatus.Cancelled, loan.Status);
    }
    
    [Fact]
    public void ThrowsWhenRescindLoanWithCertainStatus()
    {
        var loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.NotStarted, new Amount(1, 0, 0, 0, 0));

        Assert.Throws<InvalidOperationException>(() => loan.Rescind());
        
        loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.Closed, new Amount(1, 0, 0, 0, 0));
        Assert.Throws<InvalidOperationException>(() => loan.Rescind());
        
        loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.Rescinded, new Amount(1, 0, 0, 0, 0));
        Assert.Throws<InvalidOperationException>(() => loan.Rescind());
    }
    
    [Fact]
    public void CanRescindOpenLoan()
    {
        var loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.Open, new Amount(1, 0, 0, 0, 0));
        loan.Rescind();
        Assert.Equal(LoanStatus.Rescinded, loan.Status);
    }
    
    [Fact]
    public void ThrowsWhenClosingForAccountingWithBalance()
    {
        var loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.Open, new Amount(1, 0, 0, 0, 0));
        Assert.Throws<InvalidOperationException>(() => loan.CloseLoan());
    }
    
    [Fact]
    public void ThrowsWhenClosingForAccountingTwice()
    {
        var loan = new Loan(1, DateTime.Now, DateTime.Now, DateTime.Now, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.Open, new Amount(0, 0, 0, 0, 0));
        Assert.Throws<InvalidOperationException>(() => loan.CloseLoan());
    }

    [Fact]
    public void CanCloseLoan()
    {
        var loan = new Loan(1, DateTime.Now, null,null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.Open, new Amount(0, 0, 0, 0, 0));
        loan.LoanInfo = new LoanInfo(false);
        loan.CloseLoan();
        Assert.NotNull(loan.ClosedOn);
        Assert.NotNull(loan.AccountingClosedOn);
        Assert.Equal(LoanStatus.Closed, loan.Status);
    }
    
    [Fact]
    public void ThrowsWhenClosingForClosedLoan()
    {
        var loan = new Loan(1, DateTime.Now, DateTime.Now, null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.Open, new Amount(0, 0, 0, 0, 0));
        Assert.Throws<InvalidOperationException>(() => loan.CloseLoan());
    }
    
    [Fact]
    public void PartiallyClosesLoanWithPendingAch()
    {
        var loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.Open, new Amount(0, 0, 0, 0, 0));
        loan.LoanInfo = new LoanInfo(true);
        loan.CloseLoan();
        Assert.NotNull(loan.ClosedOn);
        Assert.Equal(LoanStatus.Closed, loan.Status);
    }
}