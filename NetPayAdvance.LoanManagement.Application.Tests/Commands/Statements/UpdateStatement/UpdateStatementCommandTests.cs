using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.UpdateStatement;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Common;
using NetPayAdvance.LoanManagement.Application.Services;
using NetPayAdvance.LoanManagement.Application.Users;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using Xunit;

namespace NetPayAdvance.LoanManagement.Application.Tests.Commands.Statements.UpdateStatement;

public class UpdateStatementCommandTests
{
    private readonly Mock<ILoanRepository> repo = new();
    private readonly Mock<IStatementRepository> statementRepo = new();
    private readonly Mock<IHistoryNotesService> history = new();
    private readonly Mock<IUserService> user = new();
    private readonly UpdateStatementCommandHandler handler;

    public UpdateStatementCommandTests()
    {
        user.Setup(m => m.GetUser()).Returns(new User("TST"));
        repo.Setup(m => m.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(GetMockById(1));
        handler = new UpdateStatementCommandHandler(repo.Object, statementRepo.Object, user.Object, history.Object);
    }

    private static Statement GetMockById(StatementId statementId)
    {
        var amount = Amount.Zero.AddPrincipal(100).AddInterest(100).AddCabFees(100).AddLateFees(100).AddNsf(100);
        return new Statement(statementId, amount, amount,
            new Period(DateOnly.MinValue.AddYears(2000), DateOnly.MinValue.AddMonths(1).AddYears(2000))
            , statementId.OrigDueDate);
    }

    private static Loan GetMockById(int loanId)
    {
        var amount = Amount.Zero.AddPrincipal(100).AddInterest(100).AddCabFees(100).AddLateFees(100).AddNsf(100);
        return new Loan(loanId, DateTime.Now, null, null, 
            new LoanConfig(100, 0.01m, 0, LoanType.InterestBearing, 700),
            LoanStatus.Open, amount);
    }

    [Fact]
    public async Task CannotProcessNonExtensions()
    {
        var stmtId = new StatementId(1, new DateOnly(2022, 12, 25));
        statementRepo.Setup(m => m.GetByIdAsync(stmtId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StatementId statementId, CancellationToken _) => GetMockById(statementId));
        var request = new UpdateStatementCommand(stmtId, null);
        await Assert.ThrowsAsync<UnprocessableException>(() => handler.Handle(request));
    }

    [Fact]
    public async Task UpdateLeavesNote()
    {
        var stmtId = new StatementId(1, new DateOnly(2022, 12, 25));
        statementRepo.Setup(m => m.GetByIdAsync(stmtId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StatementId statementId, CancellationToken _) => GetMockById(statementId));
        await handler.Handle(new UpdateStatementCommand(stmtId, 2));
        history.Verify(mock => mock.InsertNotes(It.IsAny<HistoryNotes>()), Times.Once());
    }

    [Fact]
    public async Task NoteExceptionDoesNotBubble()
    {
        var stmtId = new StatementId(1, new DateOnly(2022, 12, 25));
        statementRepo.Setup(m => m.GetByIdAsync(stmtId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StatementId statementId, CancellationToken _) => GetMockById(statementId));
        history.Setup(m => m.InsertNotes(It.IsAny<HistoryNotes>())).Throws<Exception>();
        await handler.Handle(new UpdateStatementCommand(stmtId, 2));
        history.Verify(mock => mock.InsertNotes(It.IsAny<HistoryNotes>()), Times.Once());
    }

    [Fact]
    public void ValidatorThrowsWhenLoanIdLessThanOrEqualZero()
    {
        var validator = new StatementIdValidator();
        var result = validator.TestValidate(new StatementId(0, new DateOnly(2022, 1, 1)));
        result.ShouldHaveValidationErrorFor(m => m.LoanId);
    }

    [Fact]
    public void ValidatorThrowsWhenStatementBefore2000()
    {
        var validator = new StatementIdValidator();
        var result = validator.TestValidate(new StatementId(1, new DateOnly(1900, 1, 1)));
        result.ShouldHaveValidationErrorFor(m => m.OrigDueDate);
    }

    [Fact]
    public void ValidatorWorks()
    {
        var validator = new StatementIdValidator();
        var result = validator.TestValidate(new StatementId(1, new DateOnly(2000, 2, 1)));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void UpdateStatementValidatorWorks()
    {
        var validator = new UpdateStatementCommandValidator();
        var result = validator.TestValidate(new UpdateStatementCommand(new StatementId(1, new DateOnly(2000, 2, 1)), null));
        result.ShouldNotHaveAnyValidationErrors();
    }
}