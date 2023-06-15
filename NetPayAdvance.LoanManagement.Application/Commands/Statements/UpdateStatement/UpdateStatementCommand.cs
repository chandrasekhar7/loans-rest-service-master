using MediatR;
using Microsoft.EntityFrameworkCore;
using NetPayAdvance.LoanManagement.Application.Services;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Models.Common;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;

namespace NetPayAdvance.LoanManagement.Application.Commands.Statements.UpdateStatement;

public record UpdateStatementCommand(StatementId StatementId, int? Extension) : IRequest<StatementModel>;

public class UpdateStatementCommandHandler : IRequestHandler<UpdateStatementCommand, StatementModel>
{
    private readonly ILoanRepository loanRepo;
    private readonly IHistoryNotesService history;
    private readonly IStatementRepository repo;
    private readonly IUserService userService;

    public UpdateStatementCommandHandler(ILoanRepository loanRepo, IStatementRepository repo, IUserService userService,
        IHistoryNotesService hist)
    {
        this.loanRepo = loanRepo ?? throw new ArgumentNullException(nameof(loanRepo));
        history = hist ?? throw new ArgumentNullException(nameof(hist));
        this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
        this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<StatementModel> Handle(UpdateStatementCommand request, CancellationToken t = default)
    {
        var statement = await repo.GetByIdAsync(request.StatementId) ?? throw new NotFoundException();

        if (request.Extension.HasValue)
        {
            var dueDate = statement.OrigDueDate.AddDays(request.Extension.Value);
            statement.ExtendDueDate(dueDate); // TODO: Update Loan Policy
            await repo.SaveChangesAsync(t);
            
            try
            {
                // TODO fix this. We shouldnt pull loan for this
                var transID = (await loanRepo.GetByIdAsync(statement.LoanId)).TransID;
                var notes = $"Due Date changed from {request.StatementId.OrigDueDate.ToShortDateString()} to {dueDate.ToShortDateString()}";
                await history.InsertNotes(new HistoryNotes(request.StatementId.LoanId, transID, "Statements.DueDate",
                    request.StatementId.OrigDueDate.ToShortDateString(), dueDate.ToShortDateString(),
                    userService.GetUser().Teller, notes));
            
            }
            catch (Exception e)
            {
             //   throw new ApplicationLayerException(e.Message);
            }
           
        }
        else
        {
            throw new UnprocessableException();
        }
        
        return new StatementModel(statement);
    }
}

public class UpdateStatementCommandValidator : AbstractValidator<UpdateStatementCommand>
{
    public UpdateStatementCommandValidator()
    {
        RuleFor(e => e.StatementId).SetValidator(new StatementIdValidator());
    }
}

public class StatementIdValidator : AbstractValidator<StatementId>
{
    public StatementIdValidator()
    {
        RuleFor(e => e.LoanId).GreaterThan(0).WithMessage("Invalid LoanID");
        RuleFor(e => e.OrigDueDate).GreaterThan(new DateOnly(2000, 1, 1)).NotNull()
            .WithMessage("Invalid Original Due Date");
    }
}