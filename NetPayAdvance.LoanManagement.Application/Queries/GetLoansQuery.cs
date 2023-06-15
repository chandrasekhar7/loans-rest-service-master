using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
namespace NetPayAdvance.LoanManagement.Application.Queries
{
    public class GetLoansQuery : IRequest<IReadOnlyList<LoanModel>>
    {
        public LoanFilter LoanFilter { get; }

        public GetLoansQuery(LoanFilter lf)
        {
            LoanFilter = lf;
        }

        public class GetLoansQueryHandler : IRequestHandler<GetLoansQuery, IReadOnlyList<LoanModel>>
        {
            private readonly ILoanRepository repo;

            public GetLoansQueryHandler(ILoanRepository repo)
            {
                this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            }

            public async Task<IReadOnlyList<LoanModel>> Handle(GetLoansQuery request, CancellationToken t = default) =>
                (await repo.GetAsync(request.LoanFilter, t)).Select(l => new LoanModel(l)).ToList().AsReadOnly();
        }
    }
}