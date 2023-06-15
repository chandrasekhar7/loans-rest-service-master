using System;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.PendingChanges;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;

namespace NetPayAdvance.LoanManagement.Persistence.Database
{
    public class LoanContext : DbContext, ILoanContext
    {
        private readonly IDomainEventDispatcher dispatcher;
        private readonly ILogger<LoanContext> logger;
        public LoanContext() { }

        public LoanContext(DbContextOptions<LoanContext> options, IDomainEventDispatcher dispatcher, ILogger<LoanContext> logger) : base(options)
        {
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public IDbConnection Connection { get; }
       
        public bool HasChanges { get; }
        
        public virtual DbSet<Loan> Loans { get; set; } = default!;
        public DbSet<LoanBalance> LoanBalances { get;  set; } = default!;
        public DbSet<StatementBalance> StatementBalances { get;  set; } = default!;
        public DbSet<AdjustmentAggregate> AdjustmentAggregates { get; set; } = default!;
        public virtual DbSet<Statement> Statements { get;  set; } = default!;
        public virtual DbSet<BillingStatement> BillingStatements { get;  set; } = default!;
        public virtual DbSet<LoanAdjustment> LoanAdjustments { get;  set; } = default!;
        public virtual DbSet<StatementAdjustment> StatementAdjustments { get;  set; } = default!;
        public virtual DbSet<ProjectedPayment> ProjectedPayments { get;  set; } = default!;
        
        public virtual DbSet<LoanContracts> LoanContracts { get;  set; } = default!;
        
        public virtual DbSet<LoanPermissions> LoanPermissions { get;  set; } = default!;
        
        public virtual DbSet<Authorization> Authorizations { get;  set; } = default!;
        
        public virtual DbSet<AdjustmentCode> AdjustmentCodes { get;  set; } = default!;
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //optionsBuilder.UseSqlServer("Data Source=172.16.1.3;Initial Catalog=paydayflex;Persist Security Info=True");
            }
            // You don't actually ever need to call this
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) => modelBuilder.ApplyConfigurationsFromAssembly(typeof(LoanContext).Assembly);

        public override int SaveChanges()
        {
            PostSaveChanges().GetAwaiter().GetResult();
            var res = base.SaveChanges();
            return res;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = await base.SaveChangesAsync(cancellationToken);
            await PostSaveChanges();
            return res;
        }

        private async Task PostSaveChanges()
        {
            await DispatchDomainEvents();
        }

        private async Task DispatchDomainEvents()
        {
            var domainEventEntities = ChangeTracker.Entries<DomainEntity>()
                .Select(po => po.Entity)
                .Where(po => po.DomainEvents.Any())
                .ToArray();

            foreach (var entity in domainEventEntities)
            {
                while (entity.DomainEvents.TryTake(out var ev))
                {
                    try
                    {
                        await dispatcher.Dispatch(ev);
                    }
                    catch(Exception ex)
                    {
                        logger.Log(LogLevel.Error, ex, ex.Message);
                        // Ignore domain events. We need to fix this eventually and log them somewhere
                    }
                    
                }
            }
        }

    }
}
