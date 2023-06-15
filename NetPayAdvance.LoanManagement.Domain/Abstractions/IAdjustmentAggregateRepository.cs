using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Domain.Abstractions;

public interface IAdjustmentAggregateRepository
{
    Task<AdjustmentAggregate?> GetLoanAdjustmentById(int loanId, CancellationToken t = default);

    Task<AdjustmentAggregate?> GetByIdAsync(int loanId, CancellationToken t = default);
    
    Task<AdjustmentAggregate?> GetByIdAsync(StatementId statementId, CancellationToken t = default);

    Task<AdjustmentAggregate?> GetByIdWithAdjustments(StatementId statementId, CancellationToken t = default);

    Task SaveChangesAsync(CancellationToken t = default);
}