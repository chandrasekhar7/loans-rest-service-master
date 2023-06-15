using System;
using System.Linq;
using AutoFixture;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;

namespace NetPayAdvance.LoanManagement.Persistence.Tests.Fixtures;

public static class TestData
{
    
    // public Loan[] Loans => new []
    // {
    //     new Loan(1, new DateTime(2020,1,1), new DateTime(2020,6,1), new DateTime(2020,6,1),
    //         new LoanConfig(0,0.1,5,LoanType.InterestBearing, 701)),
    //     new Loan(1, new DateTime(2020,1,1), new DateTime(2020,6,1), new DateTime(2020,6,1),
    //         new LoanConfig(0,0.1,5,LoanType.InterestBearing, 701)),
    // }
    
    public static Loan[] Loans { get; }

    static TestData()
    {
        // Doesnt work because of primary key issues
        Loans = new Fixture().CreateMany<Loan>(10).ToArray();
    }
    // public virtual DbSet<Loan> Loans { get; set; } = default!;
    // public DbSet<LoanBalance> LoanBalances { get;  set; } = default!;
    // public DbSet<StatementBalance> StatementBalances { get;  set; } = default!;
    // public DbSet<AdjustmentAggregate> AdjustmentAggregates { get; set; } = default!;
    // public virtual DbSet<Statement> Statements { get;  set; } = default!;
    // public virtual DbSet<BillingStatement> BillingStatements { get;  set; } = default!;
    // public virtual DbSet<LoanAdjustment> LoanAdjustments { get;  set; } = default!;
    // public virtual DbSet<StatementAdjustment> StatementAdjustments { get;  set; } = default!;
    // public virtual DbSet<ProjectedPayment> ProjectedPayments { get;  set; } = default!;
    // public virtual DbSet<Authorizations> Authorizations { get;  set; } = default!;
}