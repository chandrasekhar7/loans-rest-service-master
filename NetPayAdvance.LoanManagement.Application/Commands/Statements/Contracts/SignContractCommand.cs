using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.UpdateStatement;
using NetPayAdvance.LoanManagement.Application.Models.Common;
using NetPayAdvance.LoanManagement.Application.Models.Contracts;
using NetPayAdvance.LoanManagement.Application.Models.Inputs;
using NetPayAdvance.LoanManagement.Application.Services;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.PendingChanges;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;

namespace NetPayAdvance.LoanManagement.Application.Commands.Statements.Contracts;

public record SignContractCommand(StatementId StatementId, PostContractModel ContractModel) : IRequest<Contract>;

public class SignContractCommandHandler : IRequestHandler<SignContractCommand, Contract>
{
    private readonly ILoanRepository repo;
    private readonly IHistoryNotesService notes;
    private readonly ISkipPaymentService skipService;
    private readonly IAuthorizationRepository authRepo;
    private readonly IUserService userService;
    private readonly IContractService contractService;
    private readonly IAdjustmentAggregateRepository aggRepo;
    
    public SignContractCommandHandler(ILoanRepository repo, IContractService cont, IAdjustmentAggregateRepository adj,
        ISkipPaymentService sp, IHistoryNotesService no, IAuthorizationRepository agg, IUserService user)
    {
        contractService = cont ?? throw new ArgumentNullException(nameof(cont));
        skipService = sp ?? throw new ArgumentNullException(nameof(sp));
        this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
        notes = no ?? throw new ArgumentNullException(nameof(no));
        authRepo = agg ?? throw new ArgumentNullException(nameof(agg));
        aggRepo = adj ?? throw new ArgumentNullException(nameof(adj));
        userService = user ?? throw new ArgumentNullException(nameof(user));
    }

    public async Task<Contract> Handle(SignContractCommand request, CancellationToken t = default)
    {
        try
        {
            var stmtId = request.StatementId;
            var authorization = await authRepo.GetByFilterAsync(stmtId.LoanId,stmtId.OrigDueDate.ToString("MM/dd/yyyy"),t) ??
                                 throw new ApplicationLayerException("Couldn't find the skip payment changes");
            var teller = userService.GetUser().Teller;

            var skipPayment = new SkipChange(DateOnly.Parse(authorization.NewValue), authorization.CreatedBy);
           
            var loan = await repo.GetByIdAsync(request.StatementId.LoanId,t) ?? throw new NotFoundException();
            var period = await skipService.GetPeriod(loan);
            var aggregate = await aggRepo.GetByIdAsync(request.StatementId.LoanId, t) ?? throw new NotFoundException();
            var statement = aggregate.Statements.FirstOrDefault(x => x.OrigDueDate == stmtId.OrigDueDate) ?? throw new NotFoundException();
            var projected = loan.ProjectedPayments.FirstOrDefault(x => x.OrigDueDate == stmtId.OrigDueDate) ?? throw new NotFoundException();
          
            await notes.InsertNotes(new HistoryNotes(loan.LoanId, loan.TransID,"SkipPayment",null,null,teller, "Skip Payment Authorization Received"));

            loan.SkipPayment(stmtId.OrigDueDate, period,statement.Period.StartDate > DateOnly.FromDateTime(DateTime.Now) ? projected.Amount : statement.Balance.Amount);
            aggregate.ApplySkipPayment(stmtId.OrigDueDate, teller);

            var contract = await contractService.SignContract(loan, request.ContractModel.Contract, request.ContractModel.Signature,t);
            authorization.CompletedOn = DateTime.Now;
            authorization.CompletedBy = teller;

            await repo.SaveChangesAsync(t);
            await authRepo.SaveChangesAsync(t);
            await aggRepo.SaveChangesAsync(t);
        
            await notes.InsertNotes(new HistoryNotes(loan.LoanId, loan.TransID, "SkipPayment", skipPayment.OrigDueDate.ToShortDateString(),
                period.EndDate.ToShortDateString(), teller, $"Payment due {skipPayment.OrigDueDate.ToShortDateString()} skipped per agreement"));

            return contract;
        }
        catch (Exception e)
        {
            throw new ApplicationLayerException(e.Message);
        }
    }
}

public class SignContractCommandValidator : AbstractValidator<SignContractCommand>
{
    public SignContractCommandValidator()
    {
        RuleFor(e => e.ContractModel).SetValidator(new PostContractModelValidator());
        RuleFor(e => e.StatementId).SetValidator(new StatementIdValidator());
    }
}

public class PostContractModelValidator : AbstractValidator<PostContractModel>
{
    public PostContractModelValidator()
    {
        RuleFor(e => e.Signature).NotNull().NotEmpty().WithMessage("Invalid Signature");
        RuleFor(e => e.Contract).NotNull().WithMessage("Invalid Contract");
        RuleFor(e => e.Contract.CSS).NotEmpty().WithMessage("Invalid Contract");
        RuleFor(e => e.Contract.Start).NotEmpty().WithMessage("Invalid Contract");
        RuleFor(e => e.Contract.End).NotEmpty().WithMessage("Invalid Contract");
        RuleFor(e => e.Contract.PageBreak).NotEmpty().WithMessage("Invalid Contract");
        RuleFor(e => e.Contract.Sections).NotNull().NotEmpty().WithMessage("Invalid Contract Sections");
    }
}

