using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Application.Queries;

public record GetStatementQuery(StatementId StatementId) : IRequest<StatementModel>;

public class GetStatementQueryHandler : IRequestHandler<GetStatementQuery, StatementModel>
{
    private readonly IStatementRepository repo;

    public GetStatementQueryHandler(IStatementRepository repo)
    {
        this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }
    public async Task<StatementModel> Handle(GetStatementQuery request, CancellationToken cancellationToken)
    {
        return new StatementModel(await repo.GetByIdAsync(request.StatementId, cancellationToken) ?? throw new NotFoundException());
    }
}