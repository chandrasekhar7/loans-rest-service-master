using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Queries;

public record GetStatementsQuery(int? LoanId, bool? HasBalance, DateOnly? EndDate) : IRequest<IReadOnlyList<StatementModel>>;

public class GetStatementsQueryHandler : IRequestHandler<GetStatementsQuery, IReadOnlyList<StatementModel>>
{
    private readonly IStatementRepository repo;

    public GetStatementsQueryHandler(IStatementRepository repo)
    {
        this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public async Task<IReadOnlyList<StatementModel>> Handle(GetStatementsQuery request,
        CancellationToken t = default) => (await repo.GetAsync(new StatementFilter() { LoanId = request.LoanId, Balance = request.HasBalance, 
            EndDate = request.EndDate}, t))
        .Select(l => new StatementModel(l)).ToList().AsReadOnly();
}