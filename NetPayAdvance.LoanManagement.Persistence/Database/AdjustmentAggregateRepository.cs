using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Persistence.Database;

public class AdjustmentAggregateRepository : IAdjustmentAggregateRepository
{
    private readonly ILoanContext context;

    public AdjustmentAggregateRepository(ILoanContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<AdjustmentAggregate?> GetLoanAdjustmentById(int loanId, CancellationToken t = default) {
        var loan = await context.AdjustmentAggregates
                .Include(x => x.Loan)
                .Include(x => x.Adjustments)
                .ThenInclude(x => x.StatementAdjustments)
                .FirstOrDefaultAsync(x => x.LoanId == loanId, t);


        return loan;
    }


    public async Task<AdjustmentAggregate?> GetByIdAsync(int loanId, CancellationToken t = default)
    {
        var loan = await context.AdjustmentAggregates
            .Include(x => x.Loan)
            .ThenInclude(x => x.Balance)
            .Include(x => x.Statements)
            .ThenInclude(x => x.Balance)
            .FirstOrDefaultAsync(x => x.LoanId == loanId, t);

        if (loan != null)
        {
            context.Entry(loan.Loan.Balance).State = EntityState.Detached;
            loan.Statements.ForEach(l => context.Entry(l.Balance).State = EntityState.Detached);
        }

        return loan;
    }

    public async Task<AdjustmentAggregate?> GetByIdAsync(StatementId stmtId, CancellationToken t = default)
    {
        var loan = await context.AdjustmentAggregates
            .Include(x => x.Loan)
            .ThenInclude(x => x.Balance)
            .Include(x => x.Statements.Where(s => s.LoanId == stmtId.LoanId && s.OrigDueDate == stmtId.OrigDueDate))
            .ThenInclude(x => x.Balance)
            .FirstOrDefaultAsync(x => x.LoanId == stmtId.LoanId, t);

        if (loan != null)
        {
            context.Entry(loan.Loan.Balance).State = EntityState.Detached;
            loan.Statements.ForEach(l => context.Entry(l.Balance).State = EntityState.Detached);
        }

        return loan;
    }

    public async Task<AdjustmentAggregate?> GetByIdWithAdjustments(StatementId statementId, CancellationToken t = default)
    {
        var statement = await context.Statements.Include(x => x.Balance)
                                     .FirstOrDefaultAsync(x => x.LoanId == statementId.LoanId && x.OrigDueDate == statementId.OrigDueDate,t);
        if (statement == null)
        {
            return null;
        }
        var l = await context.Loans.Include(x => x.Balance).FirstOrDefaultAsync(x => x.LoanId == statement.LoanId,t);
        var adj = await context.LoanAdjustments.Include(x => x.StatementAdjustments).Where(x => x.LoanID == l.LoanId).ToListAsync(t);
        var loan = new AdjustmentAggregate(l, new List<Statement>() {statement}, adj);
        
        if (loan != null)
        {
            context.Entry(loan.Loan.Balance).State = EntityState.Detached;
            loan.Statements.ForEach(x => context.Entry(x.Balance).State = EntityState.Detached);
        }

        return loan;
    }

    public Task SaveChangesAsync(CancellationToken t = default) => context.SaveChangesAsync(t);
}