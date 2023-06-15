using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Domain.Abstractions;

public interface IStatementRepository
{
    Task<Statement?> GetByIdAsync(StatementId id, CancellationToken t = default);
    
    Task<IReadOnlyList<Statement>> GetAsync(StatementFilter filter, CancellationToken t = default);

    Task SaveChangesAsync(CancellationToken t = default);
}