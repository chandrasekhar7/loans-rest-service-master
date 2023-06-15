using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Domain.Entity.PendingChanges;

namespace NetPayAdvance.LoanManagement.Domain.Abstractions;

public interface IAuthorizationRepository
{
    Task<Authorization> GetByFilterAsync(int loanId, string dueDate, CancellationToken t = default);
    
    Task<IReadOnlyList<Authorization>> GetByLoanIdAsync(int loanId, CancellationToken t = default);
    
    Task SaveChangesAsync(CancellationToken t = default);

    Task AddChangesAsync(Authorization auth, CancellationToken t = default);
}