using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using System;
using System.Collections.Generic;
using System.Linq;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using Xunit;

namespace NetPayAdvance.LoanManagement.Domain.Tests.Adjustments
{
    public class AdjustmentAggregateTests
    {
        private static Loan GetDefault => new(1, DateTime.Now, null, null,
            new LoanConfig(500, 0.1m, 0.5m, LoanType.LineOfCredit, 702), LoanStatus.Open,
            new Amount(500, 10, 50, 0, 0));
        
        private static Loan GetInstDefault => new(1, DateTime.Now, null, null,
            new LoanConfig(500, 0.1m, 0.5m, LoanType.InterestBearing, 701), LoanStatus.Open, 
             new Amount(500, 10, 50, 0, 0));

        private readonly List<Statement> defaultStatements = new ()
        {
            new Statement(new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(16)),
                new Amount(100, 10, 50, 0, 0),
                new Amount(100, 10, 50, 0, 0),
                new Period(DateOnly.FromDateTime(DateTime.Now).AddDays(-5), DateOnly.FromDateTime(DateTime.Now).AddDays(16)),
                DateOnly.FromDateTime(DateTime.Now).AddDays(30))
        };
        
        [Fact]
        public void AssignsProperLoanId()
        {
            var loan = new AdjustmentAggregate(GetDefault, defaultStatements);
            Assert.Equal(loan.LoanId, GetDefault.LoanId);
        }
        
        [Fact]
        public void ThrowsWhenDebitShouldAffectStatement()
        {
            var loan = new AdjustmentAggregate(GetDefault, defaultStatements);
            Assert.Throws<InvalidOperationException>(() => loan.ApplyDebit(Debit.CorrectionDecrease, new Amount(-100, -5, -5, 0,0), "ZZZ"));
        }

        [Fact]
        public void DebitChangesBalance()
        {
            var loan = new AdjustmentAggregate(GetDefault, defaultStatements);
            loan.ApplyDebit(Debit.CorrectionDecrease, new Amount(-100, -5, -5, 0,0), "ZZZ", defaultStatements.First().StatementId);
            Assert.Equal(new Amount(400, 5, 45, 0,0), loan.Loan.Balance.Amount);
        }
        
        [Fact]
        public void DebitChangesBalanceWithNoStatement()
        {
            var loan = new AdjustmentAggregate(GetDefault, new List<Statement>());
            loan.ApplyDebit(Debit.Payment, new Amount(-100, -5, -5, 0,0), "ZZZ");
            Assert.Equal(new Amount(-100, -5, -5, 0,0), loan.Adjustments.First().Adjustment.Amount);
            Assert.Equal(new Amount(400, 5, 45, 0,0), loan.Loan.Balance.Amount);
            Assert.Single(loan.Adjustments);
        }
        
        [Fact]
        public void CreditChangesBalance()
        {
            var loan = new AdjustmentAggregate(GetDefault, defaultStatements);
            loan.ApplyCredit(Credit.CorrectionIncrease, new Amount(100, 5, 5, 0,0), "ZZZ");
            Assert.Equal(new Amount(600, 15, 55, 0,0), loan.Loan.Balance.Amount);
        }

        [Fact]
        public void DebitChangesStatementBalance()
        {
            var loan = new AdjustmentAggregate(GetDefault, defaultStatements);
            loan.ApplyDebit(Debit.CorrectionDecrease, new Amount(-100, -5, -5, 0, 0), "ZZZ", defaultStatements.First().StatementId);
            Assert.Equal(new Amount(0, 5, 45, 0,0), loan.Statements.First().Balance.Amount);
        }
        
        [Fact]
        public void CreditChangesStatementBalance()
        {
            var loan = new AdjustmentAggregate(GetDefault, defaultStatements);
            loan.ApplyCredit(Credit.CorrectionIncrease, new Amount(100, 5, 5, 0,0), "ZZZ",defaultStatements.First().StatementId);
            Assert.Equal(new Amount(600, 15, 55, 0,0), loan.Loan.Balance.Amount);
            Assert.Equal(new Amount(200, 15, 55, 0,0), loan.Statements.First().Balance.Amount);
        }

        [Fact]
        public void PaymentMustBePositiveValue()
        {
            var loan = new AdjustmentAggregate(GetDefault, defaultStatements);
            Assert.Throws<InvalidOperationException>(() => loan.ApplyPayment(1, -300, "ZZZ"));
            Assert.Throws<InvalidOperationException>(() => loan.ApplyDisbursement(1, -300, "ZZZ"));
        }
        
        [Fact]
        public void PaymentAppliesToMultipleStatementsInOrder()
        {
            var stmts = defaultStatements.ToList();
            stmts.AddRange(new[]
            {
                new Statement(new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(32)),
                    new Amount(100, 10, 50, 0, 0), new Amount(100, 0, 0, 0, 0),
                    new Period(DateOnly.FromDateTime(DateTime.Now.AddDays(16)), DateOnly.FromDateTime(DateTime.Now).AddDays(32)),
                    DateOnly.FromDateTime(DateTime.Now).AddDays(60))
            });
            var loan = new AdjustmentAggregate(GetDefault, stmts);
            loan.ApplyPayment(1, 300, "ZZZ");
            Assert.Equal(new Amount(260, 0, 0, 0, 0), loan.Loan.Balance.Amount);
            Assert.Equal(new Amount(0, 0, 0, 0, 0),loan.Statements.First().Balance.Amount);
            Assert.Equal(new Amount(0, 0, 0, 0, 0),loan.Statements[1].Balance.Amount);
        }

        [Fact]
        public void OverPaymentAppliesToPrincipal()
        {
            var loan = new AdjustmentAggregate(GetDefault, defaultStatements);
            loan.ApplyPayment(1, 200, "ZZZ");
            Assert.Equal(new Amount(360, 0, 0, 0, 0), loan.Loan.Balance.Amount);
            Assert.Equal( new Amount(0, 0, 0, 0, 0),loan.Statements.First().Balance.Amount);
        }

        [Fact]
        public void ClearsBalanceWhenLow()
        {
            var loan = new AdjustmentAggregate(GetDefault, new List<Statement>());
            loan.ApplyDebit(Debit.Discount, new Amount(-496, -10, -50, 0, 0), "ZZZ");
            Assert.True(loan.Loan.Balance.Amount == Amount.Zero);
        }

        [Fact]
        public void SuccessfulLOCPayment()
        {
            var loan = new AdjustmentAggregate(GetDefault, new List<Statement>());
            var currentBalance = loan.Loan.Balance.Amount;
            loan.ApplyPayment(1, 10, "NMR");
            Assert.True(loan.Loan.Balance.Amount.Total == currentBalance.Total - 10);
        }

        [Fact]
        public void FailedLOCPayment()
        {
            var loan = new AdjustmentAggregate(GetDefault, new List<Statement>());
            var currentBalance = loan.Loan.Balance.Amount;
            loan.ApplyPayment(1, 0, "NMR");
            Assert.True(loan.Loan.Balance.Amount.Total == currentBalance.Total);
        }

        [Fact]
        public void SuccessfulInstPayment()
        {
            var loan = new AdjustmentAggregate(GetInstDefault, new List<Statement>());
            var currentBalance = loan.Loan.Balance.Amount;
            loan.ApplyPayment(1, 10, "NMR");
            Assert.True(loan.Loan.Balance.Amount.Total == currentBalance.Total - 10);
        }

        [Fact]
        public void FailedInstPayment()
        {
            var loan = new AdjustmentAggregate(GetInstDefault, new List<Statement>());
            var currentBalance = loan.Loan.Balance.Amount;
            loan.ApplyPayment(1, 0, "NMR");
            Assert.True(loan.Loan.Balance.Amount.Total == currentBalance.Total);
        }

        [Fact]
        public void CreatedZeroBalanceDomainEvent()
        {
            var loanAgg = new AdjustmentAggregate(GetInstDefault, new List<Statement>());
            var amount = -loanAgg.Loan.Balance.Amount;
            Assert.Equal(0, loanAgg.Loan.DomainEvents.Count);
            loanAgg.ApplyDebit(Debit.CorrectionDecrease, amount, "EC1");
            Assert.Single(loanAgg.Loan.DomainEvents);
        }

        [Fact]
        public void ApplySkipPayment()
        {
            var loan = new AdjustmentAggregate(GetInstDefault, defaultStatements);
            var skipping = loan.Statements.First(x => x.Balance.Amount.Total > 0).OrigDueDate;
            loan.ApplySkipPayment(skipping,"NMR");
        }
    }
}