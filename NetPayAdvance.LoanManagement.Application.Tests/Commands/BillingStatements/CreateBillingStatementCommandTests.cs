using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using MediatR;
using Moq;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Commands.BillingStatements;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.BillingStatements;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Persistence.Tests.Fixtures;
using Xunit;

namespace NetPayAdvance.LoanManagement.Application.Tests.Commands.BillingStatements;

public class CreateBillingStatementCommandTests : BaseTestContext
{
    private readonly AdjustmentAggregate agg = new(
        new Loan(1, DateTime.Now, null, null, new LoanConfig(1m, 1m, 1m, LoanType.Payday, 700), LoanStatus.Open, Amount.Zero),
        new List<Statement>()
        {
            new(new StatementId(1, new DateOnly(2022, 1, 1)), Amount.Zero, Amount.Zero,
                new Period(new DateOnly(2021, 12, 1), new DateOnly(2022, 1, 1)), new DateOnly(2022, 1, 1))
        });

    [Fact]
    public async Task ThrowsWhenCreatingExistingStatement()
    {
        var mockContext = DefaultContext;
        var mockContractService = new Mock<IContractService>();
        var mockAdjustmentAggRepo = new Mock<IAdjustmentAggregateRepository>();
        var stmtId = new StatementId(1, new DateOnly(2022, 1, 1));
        mockContext.BillingStatements.Add(new BillingStatement(stmtId, "<html>stuff</html>"));
        mockAdjustmentAggRepo
            .Setup(m => m.GetByIdWithAdjustments(It.IsAny<StatementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(agg);

        var handler = new CreateBillingStatementCommandHandler(mockContext, mockContractService.Object, mockAdjustmentAggRepo.Object);

        await Assert.ThrowsAsync<ConflictException>(() => handler.Handle(new CreateBillingStatementCommand(stmtId)));
    }

    [Fact]
    public async Task ThrowsWhenNoStatementExists()
    {
        var mockContext = DefaultContext;
        var mockContractService = new Mock<IContractService>();
        var mockAdjustmentAggRepo = new Mock<IAdjustmentAggregateRepository>();
        var stmtId = new StatementId(1, new DateOnly(2022, 1, 1));
        var handler = new CreateBillingStatementCommandHandler(mockContext, mockContractService.Object, mockAdjustmentAggRepo.Object);
        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new CreateBillingStatementCommand(stmtId)));
    }
    
    [Fact]
    public async Task ThrowsWhenInvalidHtmlIsUsed()
    {
        var mockContext = DefaultContext;
        var mockContractService = new Mock<IContractService>();
        var mockAdjustmentAggRepo = new Mock<IAdjustmentAggregateRepository>();
        var stmtId = new StatementId(1, new DateOnly(2022, 1, 1));

        mockAdjustmentAggRepo
            .Setup(m => m.GetByIdWithAdjustments(It.IsAny<StatementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(agg);

        var handler = new CreateBillingStatementCommandHandler(mockContext, mockContractService.Object,
            mockAdjustmentAggRepo.Object);

        var ex = await Assert.ThrowsAsync<ApplicationLayerException>(() =>
            handler.Handle(new CreateBillingStatementCommand(stmtId)));
        Assert.Equal("Cannot build billing statement", ex.Message);
    }
    
    [Fact]
    public async Task ReturnsUnitWhenSuccessful()
    {
        var mockContext = DefaultContext;
        var mockContractService = new Mock<IContractService>();
        var mockAdjustmentAggRepo = new Mock<IAdjustmentAggregateRepository>();
        var stmtId = new StatementId(1, new DateOnly(2022, 1, 1));

        mockAdjustmentAggRepo
            .Setup(m => m.GetByIdWithAdjustments(It.IsAny<StatementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(agg);
        mockContractService.Setup(m =>
                m.CreateBillingStatement(It.IsAny<BillingStatementRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("<html>FAKE BILLING STATEMENT</html>");

        var handler = new CreateBillingStatementCommandHandler(mockContext, mockContractService.Object,
            mockAdjustmentAggRepo.Object);

        var rtn = await handler.Handle(new CreateBillingStatementCommand(stmtId));

        Assert.Equal(Unit.Value, rtn);
    }

    [Fact]
    public void ValidatorWorks()
    {
        var validator = new CreateBillingStatementCommandValidator();
        var result = validator.TestValidate(new CreateBillingStatementCommand(new StatementId(1, new DateOnly(2022, 1, 1))));
        result.ShouldNotHaveAnyValidationErrors();
    }
}