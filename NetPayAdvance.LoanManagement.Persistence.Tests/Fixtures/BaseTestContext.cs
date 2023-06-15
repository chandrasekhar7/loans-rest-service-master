using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Persistence.Database;

namespace NetPayAdvance.LoanManagement.Persistence.Tests.Fixtures;

public class BaseTestContext
{
    private readonly DbContextOptions<LoanContext> options;

    protected LoanContext DefaultContext => 
        new (options, new Mock<IDomainEventDispatcher>().Object, new Mock<ILogger<LoanContext>>().Object);

    protected BaseTestContext()
    {
        options = new DbContextOptionsBuilder<LoanContext>().UseInMemoryDatabase(databaseName: "temp").Options;
    }
}