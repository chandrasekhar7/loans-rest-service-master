using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Application.Commands.Adjustments;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Domain.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Queries;

public record GetAdjustmentsQuery(int LoanId) : IRequest<IEnumerable<AdjustmentModel>>;

public class GetAdjustmentsQueryHandler : IRequestHandler<GetAdjustmentsQuery, IEnumerable<AdjustmentModel>>
{
    private readonly IAdjustmentRepository facade;

    public GetAdjustmentsQueryHandler(IAdjustmentRepository fa)
    {
        facade = fa ?? throw new ArgumentNullException(nameof(fa));
    }

    public async Task<IEnumerable<AdjustmentModel>> Handle(GetAdjustmentsQuery request, CancellationToken t = default)
    {
        return (await facade.GetAsync(new AdjustmentFilter() { LoanId = request.LoanId})).Select(a => new AdjustmentModel(a));
    }
}