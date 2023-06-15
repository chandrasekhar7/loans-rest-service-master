using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;
using System.Linq;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.UpdateStatement;
using NetPayAdvance.LoanManagement.Application.Models.BillingStatements;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Domain.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Commands.BillingStatements;

public record CreateBillingStatementCommand(StatementId StatementId) : IRequest;

public class CreateBillingStatementCommandHandler : IRequestHandler<CreateBillingStatementCommand>
{
    private readonly IContractService createService;
    private readonly ILoanContext context;
    private readonly IAdjustmentAggregateRepository adjustmentAggregate;

    public CreateBillingStatementCommandHandler(ILoanContext cont, IContractService bill, IAdjustmentAggregateRepository aa)
    {
        context = cont ?? throw new ArgumentNullException(nameof(cont));
        createService = bill ?? throw new ArgumentNullException(nameof(bill));
        adjustmentAggregate = aa ?? throw new ArgumentNullException(nameof(aa));
    }

    public async Task<Unit> Handle(CreateBillingStatementCommand request, CancellationToken t = default)
    {
        var entity = await context.BillingStatements.FindAsync(new object[] {request.StatementId.LoanId, request.StatementId.OrigDueDate}, t);
        if (entity != null)
        {
            throw new ConflictException("Billing Statement already exists");
        }

        var adjAg = await adjustmentAggregate.GetByIdWithAdjustments(request.StatementId, t) ?? throw new NotFoundException("Couldn't find loan information");

        var stmt = adjAg.Statements.FirstOrDefault() ?? throw new NotFoundException();

        var bsr = new BillingStatementRequest(stmt.OrigDueDate, stmt.DueDate, stmt.Period.StartDate, stmt.Period.EndDate, adjAg);
        var statementHtml = await createService.CreateBillingStatement(bsr, t);
        
        if (statementHtml == null || statementHtml.PadRight(6).Substring(0, 6).Trim() != "<html>")
        {
            throw new ApplicationLayerException("Cannot build billing statement");
        }
        
        try
        {
            var bill = new BillingStatement(request.StatementId, statementHtml);
            await context.BillingStatements.AddAsync(bill, t);
            await context.SaveChangesAsync(t);
            return Unit.Value;
        }
        catch (Exception e)
        {
            throw new DataLayerException(e.Message);
        }
    }
}

public class CreateBillingStatementCommandValidator : AbstractValidator<CreateBillingStatementCommand>
{
    public CreateBillingStatementCommandValidator()
    {
        RuleFor(e => e.StatementId).SetValidator(new StatementIdValidator());
    }
}
