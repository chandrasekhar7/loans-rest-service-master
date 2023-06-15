using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Application.Commands.BillingStatements;

public record GetBillingStatementCommand(StatementId StatementId) : IRequest<string>;

public class GetBillingStatementCommandHandler : IRequestHandler<GetBillingStatementCommand, string>
{
    private readonly ILoanContext context;

    public GetBillingStatementCommandHandler(ILoanContext cont)
    {
        context = cont ?? throw new ArgumentNullException(nameof(cont));
    }

    public async Task<string> Handle(GetBillingStatementCommand request, CancellationToken t = default)
    {
        try
        {
            var entity = await context.BillingStatements.FindAsync(new object[] 
                             { request.StatementId.LoanId, request.StatementId.OrigDueDate}, t) 
                ?? throw new NotFoundException("Billing Statement does not exist in the database");
            return entity.StatementHTML;
        }
        catch (Exception e)
        {
            throw new ApplicationLayerException(e.Message);
        }
    }
}

