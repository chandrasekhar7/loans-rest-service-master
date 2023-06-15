using System;
using System.Threading;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Application.Commands.BillingStatements;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using NetPayAdvance.LoanManagement.Persistence.Tests.Fixtures;
using Xunit;

namespace NetPayAdvance.LoanManagement.Application.Tests.Commands.BillingStatements;

public class GetBillingStatementCommandTests : BaseTestContext
{
    [Fact]
    public async Task BillingStatementThrows()
    {
        var context = DefaultContext;
        var stmtId = new StatementId(1, new DateOnly(2022, 1, 1));
        var handler = new GetBillingStatementCommandHandler(context);
        var ex = await Assert.ThrowsAsync<ApplicationLayerException>(() => 
            handler.Handle(new GetBillingStatementCommand(stmtId)));
        Assert.Equal("Billing Statement does not exist in the database", ex.Message);
    }

    [Fact]
    public async Task GetBillingStatement()
    {
        var context = DefaultContext;
        await context.Database.EnsureDeletedAsync(CancellationToken.None);
        await context.Database.EnsureCreatedAsync(CancellationToken.None);
        var stmtId = new StatementId(1, new DateOnly(2022, 1, 1));
        await context.BillingStatements.AddAsync(new BillingStatement(stmtId, "<html>stuff</html>"));
        var handler = new GetBillingStatementCommandHandler(context);
        var response = await handler.Handle(new GetBillingStatementCommand(stmtId));
        Assert.NotNull(response);
    }
}