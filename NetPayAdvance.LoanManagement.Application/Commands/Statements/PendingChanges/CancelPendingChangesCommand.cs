using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.UpdateStatement;
using NetPayAdvance.LoanManagement.Domain.Entity.PendingChanges;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Common;
using NetPayAdvance.LoanManagement.Application.Services;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Application.Commands.Statements.PendingChanges;

public record CancelPendingChangesCommand(StatementId StatementId) : IRequest<Authorization>;

public class CancelPendingChangesCommandHandler : IRequestHandler<CancelPendingChangesCommand, Authorization>
{
    private readonly IAuthorizationRepository repo;
    private readonly IUserService userService;
    private readonly ILoanRepository loanRepo;
    private readonly IHistoryNotesService history;
  
    public CancelPendingChangesCommandHandler(IAuthorizationRepository auth, ILoanRepository loan, IUserService user, IHistoryNotesService his)
    {
        repo = auth ?? throw new ArgumentNullException(nameof(auth));
        userService = user ?? throw new ArgumentNullException(nameof(user));
        loanRepo = loan ?? throw new ArgumentNullException(nameof(loan));
        history = his ?? throw new ArgumentNullException(nameof(his));
    }

    public async Task<Authorization> Handle(CancelPendingChangesCommand request, CancellationToken t = default)
    {
        try
        {
            var loan = await loanRepo.GetByIdAsync(request.StatementId.LoanId, t) ?? throw new NotFoundException();     // We should not pull loan for notes
            var origDate = request.StatementId.OrigDueDate.ToString("MM/dd/yyyy");
            var authorization = await repo.GetByFilterAsync(loan.LoanId,origDate,t) ?? throw new NotFoundException();
            authorization.Update(userService.GetUser().Teller);
            await repo.SaveChangesAsync(t);
            try
            {
                await history.InsertNotes(new HistoryNotes(loan.LoanId, loan.TransID, "SkipPayment", origDate, "",
                    userService.GetUser().Teller, "Pending Skip Payment for payment due " +  origDate + " cancelled"));
            }
            catch (Exception e)
            {
                // Silence this error
            }
            return authorization;
        }
        catch (Exception e)
        {
            throw new ApplicationLayerException(e.Message);
        }
    }
}

public class CancelPendingChangesCommandValidator : AbstractValidator<CancelPendingChangesCommand>
{
    public CancelPendingChangesCommandValidator()
    {
        RuleFor(e => e.StatementId).SetValidator(new StatementIdValidator());
    }
}

