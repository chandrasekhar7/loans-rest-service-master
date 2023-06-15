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

public record AddPendingChangesCommand(StatementId StatementId) : IRequest<Authorization>;

public class AddPendingChangesCommandHandler : IRequestHandler<AddPendingChangesCommand, Authorization>
{
    private readonly IAuthorizationRepository repo;
    private readonly IUserService userService;
    private readonly ILoanRepository loanRepo;
    private readonly IHistoryNotesService history;
    
    public AddPendingChangesCommandHandler(IAuthorizationRepository auth, ILoanRepository loan, IUserService user, IHistoryNotesService his)
    {
        repo = auth ?? throw new ArgumentNullException(nameof(auth));
        loanRepo = loan ?? throw new ArgumentNullException(nameof(loan));
        userService = user ?? throw new ArgumentNullException(nameof(user));
        history = his ?? throw new ArgumentNullException(nameof(his));
    }

    public async Task<Authorization> Handle(AddPendingChangesCommand request, CancellationToken t = default)
    {
        try
        {
            // Domain. Who and when can we do this?
            var origDate = request.StatementId.OrigDueDate.ToString("MM/dd/yyyy");
            var loan = await loanRepo.GetByIdAsync(request.StatementId.LoanId, t) ?? throw new NotFoundException();     // We should not pull loan for notes
            var existing = await repo.GetByFilterAsync(request.StatementId.LoanId,origDate,t);
            if (existing != null)
            {
                throw new ApplicationLayerException("There is already a skip payment scheduled for the statement");
            }
            var authorization = new Authorization();
            authorization.Create(request.StatementId, PendingChangeType.DueDate, userService.GetUser().Teller);
            await repo.AddChangesAsync(authorization, t);           
            try
            {
                await history.InsertNotes(new HistoryNotes(loan.LoanId, loan.TransID, "SkipPayment", origDate, "",
                    userService.GetUser().Teller, "Pending Skip Payment for payment due " +  origDate));
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

public class AddPendingChangesCommandValidator : AbstractValidator<AddPendingChangesCommand>
{
    public AddPendingChangesCommandValidator()
    {
        RuleFor(e => e.StatementId).SetValidator(new StatementIdValidator());
    }
}