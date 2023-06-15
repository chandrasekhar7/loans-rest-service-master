using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.PendingChanges;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Persistence.Database;

public class AuthorizationRepository : IAuthorizationRepository
{
    private readonly ILoanContext context;
    
    public AuthorizationRepository(ILoanContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Authorization> GetByFilterAsync(int loanId, string dueDate, CancellationToken t = default)
    {
        return await context.Authorizations.FirstOrDefaultAsync(x => 
            x.LoanId == loanId && x.NewValue == dueDate && x.CompletedOn == null && x.CancelledOn == null,t);
    }
    
    public async Task<IReadOnlyList<Authorization>> GetByLoanIdAsync(int loanId, CancellationToken t = default) => 
        (await context.Authorizations
        .Where(f => f.LoanId == loanId).ToListAsync(t)).AsReadOnly();
    
    public async Task SaveChangesAsync(CancellationToken t = default) => await context.SaveChangesAsync(t);

    public async Task AddChangesAsync(Authorization auth,CancellationToken t = default)
    {
        await context.Authorizations.AddAsync(auth, t);
        await SaveChangesAsync(t);
    }
}