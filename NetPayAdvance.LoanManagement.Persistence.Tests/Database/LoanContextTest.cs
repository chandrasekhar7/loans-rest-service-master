using System.Linq;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.PendingChanges;
using NetPayAdvance.LoanManagement.Persistence.Database;
using Xunit;

namespace NetPayAdvance.LoanManagement.Persistence.Tests.Database;

public class LoanContextTest
{
    private readonly DbContextOptions<LoanContext> options;

    public LoanContextTest()
    {
        var connection = new SqliteConnection("Filename=:memory:");
        connection.Open();
        options = new DbContextOptionsBuilder<LoanContext>().UseSqlite(connection).Options;
    }

    [Fact]
    public void CanReadAuthorization()
    {
        var dispatcher = new Mock<IDomainEventDispatcher>();
        var logger = new Mock<ILogger<LoanContext>>();
        using var context = new LoanContext(options, dispatcher.Object, logger.Object);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        var auth = new Authorization();
        auth.Create("1", PendingChangeType.DueDate, "NMR");
        context.Authorizations.Add(auth);
        context.SaveChanges();
        var data = context.Authorizations.First();
        Assert.NotNull(data);
    }
}