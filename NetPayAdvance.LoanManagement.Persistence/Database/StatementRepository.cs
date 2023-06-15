using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Persistence.Database;

public class StatementRepository : IStatementRepository
{
    private readonly ILoanContext context;

    public StatementRepository(ILoanContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Statement?> GetByIdAsync(StatementId id, CancellationToken t = default)
    {
        var stmt = await context.Statements
            .Include(x => x.Balance)
            .FirstOrDefaultAsync(f => f.LoanId == id.LoanId && f.OrigDueDate == id.OrigDueDate);

        if (stmt != null)
        {
            context.Entry(stmt).State = EntityState.Modified;
            context.Entry(stmt.Balance).State = EntityState.Detached;
        }

        return stmt;
    }

    public async Task<IReadOnlyList<Statement>> GetAsync(StatementFilter filter, CancellationToken t = default)
    {
        var stmts = (await context.Statements.Include(x => x.Balance)
            .Where(f => !filter.LoanId.HasValue || f.LoanId == filter.LoanId)
            .Where(f => !filter.Balance.HasValue || (filter.Balance.Value &&
                                                     f.Balance.Amount.Principal +
                                                     f.Balance.Amount.Interest +
                                                     f.Balance.Amount.CabFees +
                                                     f.Balance.Amount.Nsf +
                                                     f.Balance.Amount.LateFees > 0) || (!filter.Balance.Value &&
                f.Balance.Amount.Principal +
                f.Balance.Amount.Interest +
                f.Balance.Amount.CabFees +
                f.Balance.Amount.Nsf +
                f.Balance.Amount.LateFees == 0))
            .Where(f => !filter.EndDate.HasValue || f.Period.EndDate == filter.EndDate).ToListAsync());
        
        stmts.ForEach(l => context.Entry(l.Balance).State = EntityState.Detached);
        return stmts.AsReadOnly();
    }

    public Task SaveChangesAsync(CancellationToken t = default) => context.SaveChangesAsync(t);
}