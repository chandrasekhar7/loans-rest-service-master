using MediatR;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Queries
{
    public class GetStatementAdjustmentsQuery : IRequest<IReadOnlyList<StatementAdjustmentModel>>
    {
        public int LoanID { get; init; } = default!;

        public class GetStatementAdjustmentsQueryHandler : IRequestHandler<GetStatementAdjustmentsQuery, IReadOnlyList<StatementAdjustmentModel>>
        {
            private readonly IDbFacade facade;

            public GetStatementAdjustmentsQueryHandler(IDbFacade fa)
            {
                facade = fa ?? throw new ArgumentNullException(nameof(fa));
            }

            public async Task<IReadOnlyList<StatementAdjustmentModel>> Handle(GetStatementAdjustmentsQuery request, CancellationToken t = default)
            {
                return await facade.QueryMultipleAsync<StatementAdjustmentModel>("loan.GetLoans", new { request.LoanID, StatementAdjustments = true });
            }
        }
    }
}
