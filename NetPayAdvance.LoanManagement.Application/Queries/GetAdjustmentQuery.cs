using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using NetPayAdvance.LoanManagement.Application.Commands.Adjustments;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Domain.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Queries;

public record GetAdjustmentQuery(int AdjustmentId) : IRequest<AdjustmentModel>;

public class GetAdjustmentQueryHandler : IRequestHandler<GetAdjustmentQuery, AdjustmentModel>
{
    private readonly IAdjustmentRepository facade;

    public GetAdjustmentQueryHandler(IAdjustmentRepository fa)
    {
        facade = fa ?? throw new ArgumentNullException(nameof(fa));
    }

    public async Task<AdjustmentModel> Handle(GetAdjustmentQuery request,
        CancellationToken t = default)
    {
        var adj = await facade.GetByIdAsync(request.AdjustmentId, t);
        if (adj == null)
        {
            throw new NotFoundException();
        }
        return new AdjustmentModel(adj);
    }
}