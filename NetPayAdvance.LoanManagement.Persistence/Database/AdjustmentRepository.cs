using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Persistence.Database;

public class AdjustmentRepository : IAdjustmentRepository
{
    private readonly ILoanContext context;
    public AdjustmentRepository(ILoanContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public Task<LoanAdjustment> GetByIdAsync(int adjId, CancellationToken t = default) => context.LoanAdjustments
        .Include(x => x.StatementAdjustments)
        .FirstOrDefaultAsync(f => f.AdjustmentID == adjId, t);

    public async Task<IReadOnlyList<LoanAdjustment>> GetAsync(AdjustmentFilter filter, CancellationToken t = default) => (await context.LoanAdjustments
        .Include(x => x.StatementAdjustments)
        .Where(f => filter.LoanId == f.LoanID).ToListAsync(t)).AsReadOnly();
}