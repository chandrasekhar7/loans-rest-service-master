using System.Threading;
using System.Threading.Tasks;

namespace NetPayAdvance.LoanManagement.Domain.Abstractions;

public interface ITransactionService
{
    Task<bool> PendingAch(int loanId, CancellationToken t = default);
}