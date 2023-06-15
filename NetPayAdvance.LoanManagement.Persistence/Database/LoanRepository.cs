using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Persistence.Database;

public class LoanRepository : ILoanRepository
{
    private readonly IDbFacade facade;
    private readonly ILoanContext context;
    private readonly ITransactionService trans;

    public LoanRepository(ILoanContext context, IDbFacade facade, ITransactionService trans)
    {
        this.facade = facade ?? throw new ArgumentNullException(nameof(facade));
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.trans = trans ?? throw new ArgumentNullException(nameof(trans));
    }

    /// <summary>
    /// </summary>
    /// <param name="loanId"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public async Task<Loan?> GetByIdAsync(int loanId, CancellationToken t = default)
    {
        var loan = await context.Loans
            .Include(x => x.Balance)
            .Include(x => x.Permissions)     
            .Include(x => x.Contracts)
            .Include(x => x.ProjectedPayments)
            .FirstOrDefaultAsync(x => x.LoanId == loanId, t);

        if (loan != null)
        {
            context.Entry(loan.Balance).State = EntityState.Detached;
        }
        return loan;
    }
    
    /// <summary>
    /// </summary>
    /// <param name="loanId"></param>
    /// <param name="t"></param>
    /// <returns></returns>
    public async Task<Loan?> GetByIdWithPendingAchAsync(int loanId, CancellationToken t = default)
    {
        var loan = await context.Loans
            .Include(x => x.Balance)
            .Include(x => x.Permissions)    
            .Include(x => x.Contracts)
            .Include(x => x.ProjectedPayments)
            .FirstOrDefaultAsync(x => x.LoanId == loanId, t);

        if (loan != null)
        {
            context.Entry(loan.Balance).State = EntityState.Detached;
        }
        typeof(Loan).GetProperty("LoanInfo")
            .SetValue(loan, new LoanInfo(await trans.PendingAch(loan.LoanId, t)), null);
        return loan;
    }

    public async Task<IReadOnlyList<Loan>> GetAsync(LoanFilter filter, CancellationToken t = default)
    {
        var loans = (await context.Loans
                .Include(x => x.Balance)
                .Include(x => x.Permissions) 
                .Include(x => x.Contracts)
                .Include(x => x.ProjectedPayments)
                .Where(x => x.PowerID == filter.PowerId)
                .ToListAsync(t))
            .Where(l => filter.Statuses.Count() == 0 || filter.Statuses.Contains(GetStatus(l))).ToList();

        loans.ForEach(l => context.Entry(l.Balance).State = EntityState.Detached);
        return loans.AsReadOnly();
    }

    private LoanStatus GetStatus(Loan loan)
    {
        if (loan.IsRescinded)
        {
            return LoanStatus.Rescinded;
        }

        if (loan.IsCancelled)
        {
            return LoanStatus.Cancelled;
        }

        if (loan.StartedOn == null)
        {
            return LoanStatus.NotStarted;
        }

        if (loan.ClosedOn != null)
        {
            return LoanStatus.Closed;
        }

        return LoanStatus.Open;
    }

    public Task SaveChangesAsync(CancellationToken t = default) => context.SaveChangesAsync(t);
}