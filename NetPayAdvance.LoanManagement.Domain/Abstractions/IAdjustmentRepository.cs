using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;

namespace NetPayAdvance.LoanManagement.Domain.Abstractions;

public interface IAdjustmentRepository
{
    Task<LoanAdjustment> GetByIdAsync(int adjId, CancellationToken t = default);
    
    Task<IReadOnlyList<LoanAdjustment>> GetAsync(AdjustmentFilter filter, CancellationToken t = default);
}