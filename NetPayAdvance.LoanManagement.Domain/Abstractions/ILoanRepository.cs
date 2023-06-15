using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Domain.Abstractions;

public interface ILoanRepository
{
    Task<Loan?> GetByIdAsync(int loanId, CancellationToken t = default);
    
    Task<Loan?> GetByIdWithPendingAchAsync(int loanId, CancellationToken t = default);
    Task<IReadOnlyList<Loan>> GetAsync(LoanFilter filter, CancellationToken t = default);

    Task SaveChangesAsync(CancellationToken t = default);
}