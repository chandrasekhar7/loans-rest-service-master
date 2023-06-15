using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.UpdateStatement;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Common;
using NetPayAdvance.LoanManagement.Application.Models.Inputs;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Application.Services;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Application.Commands.Statements.CreateChargebackCommand;

public record CreateChargebackCommand(int loanId,int PaymentId) : IRequest;

public class CreateChargebackCommandHandler : IRequestHandler<CreateChargebackCommand>
{
    private readonly IChargebackService repo;
    private readonly IUserService userService;
    private readonly ILoanRepository loanRepo;

    public CreateChargebackCommandHandler(ILoanRepository loanRepo, IChargebackService repo, IUserService user)
    {
        this.loanRepo = loanRepo ?? throw new ArgumentNullException(nameof(loanRepo));
        this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
        this.userService = user ?? throw new ArgumentNullException(nameof(user));
    }

    public async Task<Unit> Handle(CreateChargebackCommand request, CancellationToken t = default)
    {
        try
        {
            var teller = userService.GetUser().Teller;
           // var loan = await loanRepo.GetByIdAsync(request.loanId, t) ?? throw new NotFoundException("Couldn't find the loan information");
            await repo.CustomerChargeback(request.PaymentId, teller);
            // return new LoanModel(loan);
            return Unit.Value;
        }
        catch (Exception e)
        {
            throw new DataLayerException(e.Message);
        }

    }
}

public class CreateChargebackCommandValidator : AbstractValidator<CreateChargebackCommand>
{
    public CreateChargebackCommandValidator()
    {
        RuleFor(e => e.PaymentId).GreaterThan(0).NotNull().WithMessage("Invalid PaymentID");
        RuleFor(e => e.loanId).GreaterThan(0).NotNull().WithMessage("Invalid LoanID");
    }
}

