using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Domain.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Queries;

public record GetLoanQuery(int LoanId) : IRequest<LoanModel>;

public class GetLoanQueryHandler : IRequestHandler<GetLoanQuery, LoanModel>
{
    private readonly ILoanRepository repo;

    public GetLoanQueryHandler(ILoanRepository repo)
    {
        this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
    }

    public async Task<LoanModel> Handle(GetLoanQuery request, CancellationToken cancellationToken) =>
        new LoanModel((await repo.GetByIdAsync(request.LoanId, cancellationToken)) ?? throw new NotFoundException());
}