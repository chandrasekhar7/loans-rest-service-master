using System;
using System.Collections.Generic;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using Xunit;

namespace NetPayAdvance.LoanManagement.Domain.Tests.Adjustments
{
    public class RescindTests
    {
        private static Loan GetInst => new(1, DateTime.Now, null, null, new LoanConfig(500, 0.1m, 0.5m, LoanType.InterestBearing, 701), LoanStatus.Open, 
            new Amount(500, 10, 50, 0, 0));

        private static Loan GetLoc => new(2, DateTime.Now, null, null, new LoanConfig(500, 0.1m, 0.5m, LoanType.LineOfCredit, 701), LoanStatus.Open,
          new Amount(500, 10, 0, 0, 0));

        private static readonly List<Statement> statements = new ()
        {
            new Statement(new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(16)),
                new Amount(100, 10, 50, 0, 0),
                new Amount(100, 10, 50, 0, 0),
                new Period(DateOnly.FromDateTime(DateTime.Now).AddDays(-5), DateOnly.FromDateTime(DateTime.Now).AddDays(16)),
                DateOnly.FromDateTime(DateTime.Now).AddDays(30))
        };

        private static readonly List<LoanAdjustment> adjustments = new()
        {
            new LoanAdjustment(1, "AL4", new Adjustment(AdjustmentType.Disbursement, new Amount(500, 10, 50, 0, 0)), new List<StatementAdjustment>(), 3)
        
        };


        private static AdjustmentAggregate instLoan = new(GetInst, statements, adjustments);


        [Fact]
        public void ThrowsForInvalidRescindAmount()
        {
            Assert.Throws<InvalidOperationException>(() => instLoan.ApplyRescind(1, -3, "NMR", 1));
        }
        
        [Fact]
        public void ThrowsForInvalidStatementId()
        {
            var sId = new StatementId(2, DateOnly.FromDateTime(DateTime.Now));
            var ex = Assert.Throws<InvalidOperationException>(() => instLoan.ApplyRescind(2, 500, "AL4", 1, sId));
            Assert.Equal($"Cannot create rescind adjustment: Statement {sId} does not exist for loan 1", ex.Message);
        }

        [Fact]
        public void ThrowsForInvalidPaymentId()
        {
            var ex = Assert.Throws<InvalidOperationException>(() => instLoan.ApplyRescind(2, 500, "AL4", 12));
            Assert.Equal("Cannot create rescind adjustment: No adjustment exists for loan ID: 1 with paymentID 12", ex.Message);
        }

        [Fact]
        public void RescindingLOCOnSameDayDoesNotAddInterest()
        {
            // Given
            AdjustmentAggregate locLoan = new(GetLoc, statements, adjustments);

            // When
            locLoan.ApplyRescind(1, 500, "AL4", 3);

            // Then
            Assert.Equal(LoanStatus.Open, locLoan.Loan.Status);
            Assert.Equal(0, locLoan.Loan.Balance.Amount.Principal);
            Assert.Equal(10, locLoan.Loan.Balance.Amount.Interest);
            Assert.Null(locLoan.Loan.ClosedOn);
            Assert.Null(locLoan.Loan.AccountingClosedOn);
        }

        [Fact]
        public void RescindingLOCAfterOneOrMoreDaysCalculatesInterest()
        {
            // Given
            adjustments[0].Adjustment.CreatedOn = DateTime.Now.AddDays(-1);
            AdjustmentAggregate locLoan = new(GetLoc, statements, adjustments);

            // When
            locLoan.ApplyRescind(1, 500, "AL4", 3);

            // Then
            Assert.Equal(LoanStatus.Open, locLoan.Loan.Status);
            Assert.Equal(0, locLoan.Loan.Balance.Amount.Principal);
            Assert.Equal(9.87m, locLoan.Loan.Balance.Amount.Interest);
            Assert.Null(locLoan.Loan.ClosedOn);
            Assert.Null(locLoan.Loan.AccountingClosedOn);
        }

        [Fact]
        public void RescindingFullPrincipalClosesNonLOCLoans()
        {
            // Given
            AdjustmentAggregate locLoan = new(GetLoc, statements, adjustments);

            // When
            instLoan.ApplyRescind(1, 500, "AL4", 3);

            // Then
            Assert.Equal(LoanStatus.Rescinded, instLoan.Loan.Status);
            Assert.Equal(0, instLoan.Loan.Balance.Amount.Total);
            Assert.Equal(0, instLoan.Loan.Balance.Amount.Interest);
            Assert.NotNull(instLoan.Loan.ClosedOn);
            Assert.NotNull(instLoan.Loan.AccountingClosedOn);
        }

        [Fact]
        public void RescindingFullPrincipalLOCDoesNotCloseLoan()
        {
            // Given
            AdjustmentAggregate locLoan = new(GetLoc, statements, adjustments);

            // When
            locLoan.ApplyRescind(1, 500, "AL4", 3);

            // Then
            Assert.Equal(LoanStatus.Open, locLoan.Loan.Status);
            Assert.Null(locLoan.Loan.ClosedOn);
            Assert.Null(locLoan.Loan.AccountingClosedOn);
        }
    }
}

