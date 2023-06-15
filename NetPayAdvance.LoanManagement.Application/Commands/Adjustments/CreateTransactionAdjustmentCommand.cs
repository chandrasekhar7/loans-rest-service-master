using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Application.Validators;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Domain.Entity.Transactions;

namespace NetPayAdvance.LoanManagement.Application.Commands.Adjustments;

public record CreateTransactionAdjustmentCommand(TransactionAdjustmentModel Adjustment) : IRequest<AdjustmentModel> {}

public class CreateTransactionAdjustmentCommandHandler : IRequestHandler<CreateTransactionAdjustmentCommand, AdjustmentModel>
{
    private readonly IAdjustmentAggregateRepository repo;

    public CreateTransactionAdjustmentCommandHandler(IAdjustmentAggregateRepository repo)
    {
        this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }
    
    public async Task<AdjustmentModel> Handle(CreateTransactionAdjustmentCommand request, CancellationToken t = default)
    {
        var trans = request.Adjustment;
        var loan = await repo.GetByIdAsync(request.Adjustment.LoanId,t) ?? throw new NotFoundException();

        if (trans.TransactionType == TransactionType.Disburse)
        {
            loan.ApplyDisbursement(trans.TransactionId, trans.Result == TransactionResult.Success ? trans.Amount : 0, trans.Teller);
        }
        else if (trans.TransactionType == TransactionType.Credit)
        {
            throw new NotImplementedException();
        }
        else if (trans.TransactionType == TransactionType.Rescind)
        {
            await repo.GetLoanAdjustmentById(request.Adjustment.LoanId);

            if(trans.RescindPaymentId == null) throw new ArgumentNullException("rescindPaymentId");
            loan.ApplyRescind(trans.TransactionId, trans.Result == TransactionResult.Success ? trans.Amount : 0, trans.Teller, trans.RescindPaymentId ?? 0, 
                trans.StatementId == null ? null : new StatementId(trans.LoanId, trans.StatementId.Value));
        }
        else if(trans.TransactionType == TransactionType.Debit)
        {
            loan.ApplyPayment(trans.TransactionId, trans.Result == TransactionResult.Success ? trans.Amount : 0, trans.Teller, 
                trans.StatementId == null ? null : new StatementId(trans.LoanId, trans.StatementId.Value));
        }
        
        await repo.SaveChangesAsync(t);
        return new AdjustmentModel(loan.Adjustments.First());
    }
}

public class CreateTransactionAdjustmentCommandValidator : AbstractValidator<CreateTransactionAdjustmentCommand>
{
    public CreateTransactionAdjustmentCommandValidator()
    {
        RuleFor(e => e.Adjustment).SetValidator(new TransactionAdjustmentModelValidator());
    }
}