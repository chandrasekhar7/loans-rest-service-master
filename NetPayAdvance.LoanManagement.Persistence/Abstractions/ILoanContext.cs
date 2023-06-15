using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.PendingChanges;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;

namespace NetPayAdvance.LoanManagement.Persistence.Abstractions
{
    public interface ILoanContext
    {
        DatabaseFacade Database { get; }
        
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        
        public DbSet<Loan> Loans { get; }
        public DbSet<AdjustmentAggregate> AdjustmentAggregates { get; }
        
        public DbSet<LoanBalance> LoanBalances { get; }
        
        public DbSet<StatementBalance> StatementBalances { get; }
        
        public DbSet<Statement> Statements { get; }
        
        public DbSet<LoanAdjustment> LoanAdjustments { get; }

        public DbSet<StatementAdjustment> StatementAdjustments { get; }
        
        public DbSet<ProjectedPayment> ProjectedPayments { get; } 
        
        public DbSet<Authorization> Authorizations { get; }

        public DbSet<BillingStatement> BillingStatements { get; }
        
        public DbSet<AdjustmentCode> AdjustmentCodes { get; }
        
        public DbSet<LoanPermissions> LoanPermissions { get; }
        
        public DbSet<LoanContracts> LoanContracts  { get; }
    } 
}

