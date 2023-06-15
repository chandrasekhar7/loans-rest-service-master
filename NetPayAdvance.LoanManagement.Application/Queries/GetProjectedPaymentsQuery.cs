using MediatR;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Queries
{
    public class GetProjectedPaymentsQuery : IRequest<IReadOnlyList<ProjectedPaymentModel>>
    {
        public int LoanID { get; init; } = default!;

        public class GetProjectedPaymentsQueryHandler : IRequestHandler<GetProjectedPaymentsQuery, IReadOnlyList<ProjectedPaymentModel>>
        {
            private readonly ILoanRepository repo;

            public GetProjectedPaymentsQueryHandler(ILoanRepository repo)
            {
                this.repo = repo ?? throw new ArgumentNullException(nameof(repo));
            }

            public async Task<IReadOnlyList<ProjectedPaymentModel>> Handle(GetProjectedPaymentsQuery request, CancellationToken t = default)
            {
                // TODO: make this leaner and dont pull in loan
                var loan = await repo.GetByIdAsync(request.LoanID) ?? throw new NotFoundException();
                return loan.ProjectedPayments.Select(p => new ProjectedPaymentModel(p)).ToList().AsReadOnly();
                //return await facade.QueryMultipleAsync<ProjectedPaymentModel>("loan.GetLoans", new { request.LoanID, Projected = true });
            }
        }
    }
}
