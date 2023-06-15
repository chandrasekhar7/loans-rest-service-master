using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Events;
using NetPayAdvance.LoanManagement.Application.Models.Common;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Application.Services;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Events;

namespace NetPayAdvance.LoanManagement.Application.Commands.Loans;

public record CloseLoanCommand(int LoanID,string Notes) : IRequest<LoanModel>;

public class CloseLoanCommandHandler : IRequestHandler<CloseLoanCommand, LoanModel>, INotificationHandler<DomainEventNotification<LoanBalanceZeroEvent>>
{
    private readonly ILoanRepository repo;
    private readonly IHistoryNotesService history;
    private readonly IUserService userService;

    public CloseLoanCommandHandler(ILoanRepository loanRepo, IHistoryNotesService hist, IUserService user)
    {
        repo = loanRepo ?? throw new ArgumentNullException(nameof(loanRepo));
        history = hist ?? throw new ArgumentNullException(nameof(hist));
        userService = user ?? throw new ArgumentNullException(nameof(user));
    }

    public Task<LoanModel> Handle(CloseLoanCommand request, CancellationToken t = default) => CloseLoan(request.LoanID, request.Notes, t);

    public Task Handle(DomainEventNotification<LoanBalanceZeroEvent> notification, CancellationToken t) => CloseLoan(notification.DomainEvent.LoanId, null, t);

    private async Task<LoanModel> CloseLoan(int loanId, string? note, CancellationToken t = default)
    {
        try
        {
            var teller = userService.GetUser().Teller;
            var loan = await repo.GetByIdWithPendingAchAsync(loanId, t) ?? throw new NotFoundException("Couldn't find the loan information");

            loan.CloseLoan();
            await repo.SaveChangesAsync(t);
       
            if (loan.Config != null && loan.Config.LoanType == LoanType.LineOfCredit)
            {
                await history.InsertNotes(new HistoryNotes(loanId, loan.TransID,
                    "Loans.CompletedOn", "", "", teller, "Close LOC: reason - "+note));
            }
            return new LoanModel(loan);
        }
        catch (Exception e)
        {
            throw new ApplicationLayerException(e.Message);
        }
    }
}

public class CloseLoanCommandValidator : AbstractValidator<CloseLoanCommand>
{
    public CloseLoanCommandValidator()
    {
        RuleFor(e => e.LoanID).NotNull().GreaterThan(0).WithMessage("Invalid Loan ID");
    }
}

