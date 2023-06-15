using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Commands.Adjustments;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Common;
using NetPayAdvance.LoanManagement.Application.Models.Inputs;
using NetPayAdvance.LoanManagement.Application.Services;
using NetPayAdvance.LoanManagement.Application.Users;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using Xunit;

namespace NetPayAdvance.LoanManagement.Application.Tests.Commands.Adjustments;

public class CreateLoanAdjustmentCommandTests
{
    private readonly Mock<IAdjustmentAggregateRepository> mockAdjustmentRepo = new();
    private readonly Mock<IHistoryNotesService> mockHistoryService = new();
    private readonly CreateLoanAdjustmentCommandHandler handler;

    public CreateLoanAdjustmentCommandTests()
    {
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(m => m.GetUser()).Returns(new User("TST"));
        handler = new CreateLoanAdjustmentCommandHandler(mockAdjustmentRepo.Object, mockHistoryService.Object, mockUserService.Object);
    }

    private static AdjustmentAggregate GetMockById(int loanId)
    {
        var amount = Amount.Zero.AddPrincipal(100).AddInterest(100).AddCabFees(100).AddLateFees(100).AddNsf(100);
        return new AdjustmentAggregate(new Loan(loanId, DateTime.Now, null, null, new LoanConfig(100, 0.01m, 0, LoanType.InterestBearing, 700),
            LoanStatus.Open, amount), new List<Statement>() { }, null);
    }

    [Fact]
    public async Task AdjustmentLeavesNote()
    {
        int loanId = 1;
        mockAdjustmentRepo.Setup(m => m.GetByIdAsync(loanId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((int loanId, CancellationToken _) => GetMockById(loanId));
        await handler.Handle(new CreateLoanAdjustmentCommand(1, new CreateAdjustmentModel()
        {
            AdjustmentType = AdjustmentType.CorrectionDecrease,
            CabFees = -10,
            Interest = -5,
            LateFees = -1,
            Notes = "",
            NSF = -1,
            Principal = -20
        }));
        mockHistoryService.Verify(mock => mock.InsertNotes(It.IsAny<HistoryNotes>()), Times.Once());
    }

    [Fact]
    public async Task InvalidAdjustmentTypeThrows()
    {
        int loanId = 1;
        mockAdjustmentRepo.Setup(m => m.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((int loanId, CancellationToken _) => GetMockById(loanId));
        mockHistoryService.Setup(m => m.InsertNotes(It.IsAny<HistoryNotes>())).Throws<Exception>();
        var ex = await Assert.ThrowsAsync<ApplicationLayerException>(() => handler.Handle(new CreateLoanAdjustmentCommand(1, new CreateAdjustmentModel()
        {
            AdjustmentType = (AdjustmentType)2000,
            CabFees = 10,
            Interest = 5,
            LateFees = 1,
            Notes = "",
            NSF = 1,
            Principal = 20
        })));
        Assert.Equal("Invalid Adjustment", ex.Message);
    }

    [Fact]
    public async Task AmountMustbeGreaterThanZeroError()
    {
        int loanId = 1;
        mockAdjustmentRepo.Setup(m => m.GetByIdAsync(1, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((int loanId, CancellationToken _) => GetMockById(loanId));
        mockHistoryService.Setup(m => m.InsertNotes(It.IsAny<HistoryNotes>())).Throws<Exception>();
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(new CreateLoanAdjustmentCommand(1, new CreateAdjustmentModel()
        {
            AdjustmentType = AdjustmentType.Payment,
            CabFees = 10,
            Interest = 5,
            LateFees = 1,
            Notes = "",
            NSF = 1,
            Principal = 20
        })));
        Assert.Equal("Amount must be less than zero", ex.Message);
    }

    [Fact]
    public void ValidatorWorks()
    {
        var adjustment = new CreateAdjustmentModel()
        {
            AdjustmentType = AdjustmentType.CorrectionDecrease,
            CabFees = -10,
            Interest = -5,
            LateFees = -1,
            Notes = "",
            NSF = -1,
            Principal = -20
        };
        var validator = new CreateLoanAdjustmentModelValidator();
        var result = validator.TestValidate((adjustment));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidatorFails()
    {
        var adjustment = new CreateAdjustmentModel()
        {
            AdjustmentType = AdjustmentType.CorrectionDecrease,
            CabFees = -10,
            Interest = -5,
            LateFees = -1,
            Notes = "",
            NSF = -1,
            Principal = -20
        };
        var validator = new CreateLoanAdjustmentCommandValidator();
        var result = validator.TestValidate(new CreateLoanAdjustmentCommand(0, adjustment));
        result.ShouldHaveAnyValidationError();
    }
}