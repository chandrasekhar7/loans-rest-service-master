using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.TestHelper;
using Moq;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.CreateChargebackCommand;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.UpdateStatement;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Common;
using NetPayAdvance.LoanManagement.Application.Services;
using NetPayAdvance.LoanManagement.Application.Users;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using Xunit;

namespace NetPayAdvance.LoanManagement.Application.Tests.Commands.Chargeback;

public class ChargebackcommandTest
{
    private readonly Mock<IChargebackService> repo = new();
    private readonly CreateChargebackCommandHandler handler;
    private readonly Mock<ILoanRepository> loanRepo = new();
    private readonly Mock<IUserService> User = new();

    public ChargebackcommandTest()
    {
        User.Setup(m => m.GetUser()).Returns(new User("SP2"));
        handler = new CreateChargebackCommandHandler(loanRepo.Object,repo.Object, User.Object);
    }
    [Fact]
    public async Task CallCreateChargebackCommand()
    {
        await handler.Handle(new CreateChargebackCommand(1, 1), CancellationToken.None);
        repo.Verify(mock => mock.CustomerChargeback(1, "SP2"), Times.Once);
    }

    [Fact]
    public void ValidatorWorks()
    {
        var validator = new CreateChargebackCommandValidator();
        var result = validator.TestValidate(new CreateChargebackCommand(1, 1));
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void ValidatorFails()
    {
        var validator = new CreateChargebackCommandValidator();
        var result = validator.TestValidate(new CreateChargebackCommand(1, 0));
        result.ShouldHaveAnyValidationError();
    }
}