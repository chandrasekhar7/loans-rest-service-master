using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NetPayAdvance.LoanManagement.Application.Commands.Adjustments;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Domain.Entity.Transactions;
using Xunit;

namespace NetPayAdvance.LoanManagement.Application.Tests.Commands.Adjustments;

public class CreateTransactionAdjustmentCommandTests
{
    private static void Mock(Mock<IAdjustmentAggregateRepository> mockRepo)
    {
        var loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(1000, 0.1m, 1m, LoanType.InterestBearing, 701),
            LoanStatus.Open, new Amount(500, 10, 10, 0, 0));

        typeof(Loan).GetProperty("LoanInfo").SetValue(loan, new LoanInfo(false), null);
        mockRepo.Setup(o => o.GetByIdAsync(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync(
            new AdjustmentAggregate(loan, new List<Statement>()
            {
                new(new StatementId(1, new DateOnly(2022, 1, 4)), Amount.Zero.AddPrincipal(90).AddInterest(10), Amount.Zero.AddPrincipal(90).AddInterest(10),
                    new Period(new DateOnly(2021, 12, 4), new DateOnly(2022, 1, 4)), new DateOnly(2022, 1, 18))
            }
        ));
    }

    [Fact]
    public async Task DebitTransactionCreatesAdjustment()
    {
        var mockRepo = new Mock<IAdjustmentAggregateRepository>();
        Mock(mockRepo);
        var cmd = new CreateTransactionAdjustmentCommand(new TransactionAdjustmentModel()
        {
            Amount = 520,
            LoanId = 1,
            StatementId = new DateOnly(2022,1,4),
            TransactionId = 2,
            TransactionType = TransactionType.Debit,
            Result = TransactionResult.Success
        });
        var result = await new CreateTransactionAdjustmentCommandHandler(mockRepo.Object).Handle(cmd, CancellationToken.None);
        result.Amount.Total.Should().Be(-cmd.Adjustment.Amount);
        result.Teller.Should().Be(cmd.Adjustment.Teller);
        result.CreatedOn.Should().BeAfter(DateTime.Today);
        result.LoanId.Should().Be(cmd.Adjustment.LoanId);
        result.PaymentId.Should().Be(cmd.Adjustment.TransactionId);
        result.AdjustmentCodeId.Should().Be(1);
    }

    [Fact]
    public async Task MakesZeroAdjustmentOnFailedPayment()
    {
        var mockRepo = new Mock<IAdjustmentAggregateRepository>();
        Mock(mockRepo);
        var cmd = new CreateTransactionAdjustmentCommand(new TransactionAdjustmentModel()
        {
            Amount = 100,
            LoanId = 1,
            StatementId = new DateOnly(2022,1,4),
            TransactionId = 2,
            TransactionType = TransactionType.Debit,
            Result = TransactionResult.Fail
        });
        var result = await new CreateTransactionAdjustmentCommandHandler(mockRepo.Object).Handle(cmd, CancellationToken.None);
        result.Amount.Should().Be(Amount.Zero);
        result.Teller.Should().Be(cmd.Adjustment.Teller);
        result.CreatedOn.Should().BeAfter(DateTime.Today);
        result.LoanId.Should().Be(cmd.Adjustment.LoanId);
        result.PaymentId.Should().Be(cmd.Adjustment.TransactionId);
        result.AdjustmentCodeId.Should().Be(1);
    }
    
    [Fact]
    public async Task MakesZeroAdjustmentOnFailedDisbursement()
    {
        var mockRepo = new Mock<IAdjustmentAggregateRepository>();
        Mock(mockRepo);
        var cmd = new CreateTransactionAdjustmentCommand(new TransactionAdjustmentModel()
        {
            Amount = 100,
            LoanId = 1,
            StatementId = new DateOnly(2022,1,4),
            TransactionId = 2,
            TransactionType = TransactionType.Disburse,
            Result = TransactionResult.Fail
        });
        var result = await new CreateTransactionAdjustmentCommandHandler(mockRepo.Object).Handle(cmd, CancellationToken.None);
        result.Amount.Should().Be(Amount.Zero);
        result.Teller.Should().Be(cmd.Adjustment.Teller);
        result.CreatedOn.Should().BeAfter(DateTime.Today);
        result.LoanId.Should().Be(cmd.Adjustment.LoanId);
        result.PaymentId.Should().Be(cmd.Adjustment.TransactionId);
        result.AdjustmentCodeId.Should().Be(64);
    }

    [Fact]
    public async Task DisburseTransactionCreatesAdjustment()
    {
        var mockRepo = new Mock<IAdjustmentAggregateRepository>();
        Mock(mockRepo);
        var cmd = new CreateTransactionAdjustmentCommand(new TransactionAdjustmentModel()
        {
            Amount = 100,
            LoanId = 1,
            StatementId = new DateOnly(2022,1,4),
            TransactionId = 2,
            TransactionType = TransactionType.Disburse,
            Result = TransactionResult.Success
        });
        var result = await new CreateTransactionAdjustmentCommandHandler(mockRepo.Object).Handle(cmd, CancellationToken.None);
        result.Amount.Total.Should().Be(cmd.Adjustment.Amount);
        result.Teller.Should().Be(cmd.Adjustment.Teller);
        result.CreatedOn.Should().BeAfter(DateTime.Today);
        result.LoanId.Should().Be(cmd.Adjustment.LoanId);
        result.PaymentId.Should().Be(cmd.Adjustment.TransactionId);
        result.AdjustmentCodeId.Should().Be(64);
    }

    [Fact]
    public async Task ThrowsWhenLoanNotFound()
    {
        var mockRepo = new Mock<IAdjustmentAggregateRepository>();
        var cmd = new CreateTransactionAdjustmentCommand(new TransactionAdjustmentModel()
        {
            Amount = 100,
            LoanId = 1,
            StatementId = new DateOnly(2022,1,4),
            TransactionId = 2,
            TransactionType = TransactionType.Debit
        });
        await Assert.ThrowsAsync<NotFoundException>(() => new CreateTransactionAdjustmentCommandHandler(mockRepo.Object).Handle(cmd, CancellationToken.None));
    }

    [Fact]
    public async Task SavesAdjustment()
    {
        var mockRepo = new Mock<IAdjustmentAggregateRepository>();
        Mock(mockRepo);
        var cmd = new CreateTransactionAdjustmentCommand(new TransactionAdjustmentModel()
        {
            Amount = 100,
            LoanId = 1,
            StatementId = new DateOnly(2022,1,4),
            TransactionId = 2,
            TransactionType = TransactionType.Disburse
        });
        await new CreateTransactionAdjustmentCommandHandler(mockRepo.Object).Handle(cmd, CancellationToken.None);
        // Throws if save was never called
        mockRepo.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task RescindLoanThrowsWithNoRescindPaymentId()
    {
        var mockRepo = new Mock<IAdjustmentAggregateRepository>();
        Mock(mockRepo);
        var cmd = new CreateTransactionAdjustmentCommand(new TransactionAdjustmentModel()
        {
            Amount = 500,
            LoanId = 1,
            TransactionId = 2,
            TransactionType = TransactionType.Rescind
        });
        
        var ex = Assert.ThrowsAsync<ArgumentNullException>( async () =>
        {
            await new CreateTransactionAdjustmentCommandHandler(mockRepo.Object).Handle(cmd, CancellationToken.None);
        });
    }
}