using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

public class CreateAdjustmentCommandTests
{
    private readonly Mock<IAdjustmentAggregateRepository> mockAdjustmentRepo = new();
    private readonly Mock<IHistoryNotesService> mockHistoryService = new();
    private readonly CreateAdjustmentCommandHandler handler;

    public CreateAdjustmentCommandTests()
    {
        var mockUserService = new Mock<IUserService>();
        mockUserService.Setup(m => m.GetUser()).Returns(new User("TST"));
        handler = new CreateAdjustmentCommandHandler(mockAdjustmentRepo.Object, mockHistoryService.Object, mockUserService.Object);
    }

    private static AdjustmentAggregate GetMockById(StatementId statementId)
    {
        var amount = Amount.Zero.AddPrincipal(100).AddInterest(100).AddCabFees(100).AddLateFees(100).AddNsf(100);
        return new AdjustmentAggregate(new Loan(statementId.LoanId,DateTime.Now,null, null, new LoanConfig(100,0.01m,0,LoanType.InterestBearing, 700), 
            LoanStatus.Open, amount), new List<Statement>() {
                new(statementId, amount, amount, new Period(DateOnly.MinValue.AddYears(2000), DateOnly.MinValue.AddMonths(1).AddYears(2000)), DateOnly.MaxValue)
        });
    }

    [Fact]
    public async Task AdjustmentLeavesNote()
    {
        var st = new StatementId(1, new DateOnly(2022, 12, 25));
        mockAdjustmentRepo.Setup(m => m.GetByIdAsync(st, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((StatementId statementId, CancellationToken _) => GetMockById(statementId));
        await handler.Handle(new CreateAdjustmentCommand(st, new CreateAdjustmentModel()
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
    public async Task NoteExceptionDoesNotBubble()
    {
        var st = new StatementId(1, new DateOnly(2022, 12, 25));
        mockAdjustmentRepo.Setup(m => m.GetByIdAsync(st, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((StatementId statementId, CancellationToken _) => GetMockById(statementId));
        mockHistoryService.Setup(m => m.InsertNotes(It.IsAny<HistoryNotes>())).Throws<Exception>();
        await handler.Handle(new CreateAdjustmentCommand(st, new CreateAdjustmentModel()
        {
            AdjustmentType = AdjustmentType.CorrectionIncrease,
            CabFees = 10,
            Interest = 5,
            LateFees = 1,
            Notes = "",
            NSF = 1,
            Principal = 20
        }));
        mockHistoryService.Verify(mock => mock.InsertNotes(It.IsAny<HistoryNotes>()), Times.Once());
    }

    [Fact]
    public async Task InvalidAdjustmentTypeThrows()
    {
        var st = new StatementId(1, new DateOnly(2022, 12, 25));
        mockAdjustmentRepo.Setup(m => m.GetByIdAsync(st, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((StatementId statementId, CancellationToken _) => GetMockById(statementId));
        mockHistoryService.Setup(m => m.InsertNotes(It.IsAny<HistoryNotes>())).Throws<Exception>();
        var ex = await Assert.ThrowsAsync<ApplicationLayerException>(() => handler.Handle(new CreateAdjustmentCommand(st, new CreateAdjustmentModel()
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
}