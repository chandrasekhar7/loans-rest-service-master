// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.Logging;
// using System.Data.Common;
// using Microsoft.Data.Sqlite;
// using Microsoft.EntityFrameworkCore.Infrastructure;
// using Moq;
// using NetPayAdvance.LoanManagement.Domain.Abstractions;
// using NetPayAdvance.LoanManagement.Persistence.Database;
//
// namespace NetPayAdvance.LoanManagement.Persistence.Tests.Fixtures
// {
//     public class DatabaseFixture
//     {
//         protected readonly LoanContext context;
//         protected readonly DbConnection _connection;
//
//         public Mock<IDomainEventDispatcher> MockDispatcher => new Mock<IDomainEventDispatcher>();
//         
//         public LoanContext Context => context;
//         
//         private static DbConnection CreateInMemoryDatabase()
//         {
//             var connection = new SqliteConnection("Filename=:memory:");
//             connection.Open();
//             return connection;
//         }
//         
//         public DatabaseFixture()
//         {
//             _connection = CreateInMemoryDatabase();
//             var options = new DbContextOptionsBuilder<LoanContext>()
//                 .UseSqlite(_connection)
//                 //.UseSqlServer("Data Source=NPATest;Initial Catalog=paydayflex;Persist Security Info=True;User ID=CLIENT1;Password=CLIENT1;Application Name=Lending;")
//                 .UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }))
//                 .Options;
//             context = new LoanContext(options, MockDispatcher.Object, new Mock<ILogger<LoanContext>>().Object);
//             context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
//             InitDb();
//         }
//         
//         private void InitDb()
//         {
//             context.Database.EnsureCreated();
//             //context.Loans.AddRange(TestData.Loans);
//             //    context.LoanEntity.Add(new Loan());
//         //    context.StatementEntity.Add(new Statement(100, 40, 5, 0, 45, DateTime.Now.AddDays(-14), DateTime.Now.AddDays(14), DateTime.Parse("08/05/2022 00:00:00"), DateTime.Now.AddDays(28)));
//             //context.SaveChanges();
//         }
//         
//         public void Dispose() => Context.Dispose();
//     }
// }
