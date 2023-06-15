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

namespace NetPayAdvance.LoanManagement.Application.Commands.Adjustments;

public record CreateAdjustmentCommand(StatementId StatementId, CreateAdjustmentModel Adjustment) : IRequest<AdjustmentModel>;

public class CreateAdjustmentCommandHandler : IRequestHandler<CreateAdjustmentCommand, AdjustmentModel>
{
    private readonly IHistoryNotesService history;
    private readonly IAdjustmentAggregateRepository repo;
    private readonly IUserService userService;

    public CreateAdjustmentCommandHandler(IAdjustmentAggregateRepository repo, IHistoryNotesService hist, IUserService userService)
    {
        history = hist ?? throw new ArgumentNullException(nameof(hist));
        this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
        this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public async Task<AdjustmentModel> Handle(CreateAdjustmentCommand request, CancellationToken t = default)
    {
        var input = request.Adjustment;
        var loanAgg = await repo.GetByIdAsync(request.StatementId,t) ?? throw new NotFoundException();
        
        var amount = new Amount(input.Principal, input.Interest, input.CabFees, input.NSF, input.LateFees);
        if (Enum.TryParse(input.AdjustmentType.ToString(), out Debit debit) && Enum.IsDefined(typeof(Debit), debit))
        {
            loanAgg.ApplyDebit(debit, amount, userService.GetUser().Teller, request.StatementId);
        }
        else if (Enum.TryParse(input.AdjustmentType.ToString(), out Credit credit) && Enum.IsDefined(typeof(Credit), credit))
        {
            loanAgg.ApplyCredit(credit, amount, userService.GetUser().Teller, request.StatementId);
        }
        else
        {
            throw new ApplicationLayerException("Invalid Adjustment");
        }
        
        await repo.SaveChangesAsync(t);

        try
        {
            await history.InsertNotes(new HistoryNotes(loanAgg.LoanId, loanAgg.Loan.TransID, 
                "Statements.Adjustment", "", "", userService.GetUser().Teller,
                input.Notes == "" ? "Manual Statement Adjustment" : input.Notes));
        }
        catch (Exception e)
        {
            // Silence this error
        }
        return new AdjustmentModel(loanAgg.Adjustments.First());
    }
}

public class CreateAdjustmentCommandValidator : AbstractValidator<CreateAdjustmentCommand>
{
    public CreateAdjustmentCommandValidator()
    {
        RuleFor(e => e.StatementId).SetValidator(new StatementIdValidator());
        RuleFor(e => e.Adjustment).SetValidator(new CreateAdjustmentModelValidator());
    }
}

public class CreateAdjustmentModelValidator : AbstractValidator<CreateAdjustmentModel>
{
    public CreateAdjustmentModelValidator()
    {
        RuleFor(e => e.Principal).NotNull().WithMessage("Invalid Principal");
        RuleFor(e => e.Interest).NotNull().WithMessage("Invalid Interest");
        RuleFor(e => e.CabFees).NotNull().WithMessage("Invalid Cab Fees");
        RuleFor(e => e.NSF).NotNull().WithMessage("Invalid NSF");
        RuleFor(e => e.LateFees).NotNull().WithMessage("Invalid Late Fees");
        RuleFor(e => e.Notes).NotNull().WithMessage("Invalid Notes");
    }
}
