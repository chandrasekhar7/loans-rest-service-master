using System;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Persistence.Tests.Fixtures;
using Xunit;

namespace NetPayAdvance.LoanManagement.Persistence.Tests.Database;

public class StatementRepositoryTests : BaseTestContext
{
    [Fact]
    public async Task AddStatementEntity()
    {
        await using var context = DefaultContext;
        var statement = new Statement(new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(16)), new Amount(100, 10, 50, 0, 0),new Amount(100, 10, 50, 0, 0),
            new Period(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now).AddDays(16)), DateOnly.FromDateTime(DateTime.Now).AddDays(30));
        await context.Statements.AddAsync(statement);
        await context.SaveChangesAsync();
    }
}