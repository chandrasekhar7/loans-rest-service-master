using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NetPayAdvance.LoanManagement.Application.Commands.Adjustments;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Queries;

public record GetAdjustmentCodesQuery() : IRequest<List<AdjustmentCode>>;

public class GetAdjustmentCodesQueryHandler : IRequestHandler<GetAdjustmentCodesQuery, List<AdjustmentCode>>
{
    private readonly ILoanContext context;

    public GetAdjustmentCodesQueryHandler(ILoanContext fa)
    {
        context = fa ?? throw new ArgumentNullException(nameof(fa));
    }

    public async Task<List<AdjustmentCode>> Handle(GetAdjustmentCodesQuery request,CancellationToken t = default)
    {
        return await context.AdjustmentCodes.ToListAsync(t);
    }
}