 using NetPayAdvance.LoanManagement.Application.Models.Contracts;
 using NetPayAdvance.LoanManagement.Infrastructure.Contracts.Services;
 using NetPayAdvance.Transactions.Tests.Helpers;
 using System;
 using System.Collections.Generic;
 using System.Data;
 using System.Net;
 using System.Net.Http;
 using System.Threading;
 using System.Threading.Tasks;
 using Microsoft.EntityFrameworkCore;
 using Microsoft.Extensions.Logging;
 using Moq;
 using NetPayAdvance.LoanManagement.Domain.Abstractions;
 using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
 using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
 using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
 using NetPayAdvance.LoanManagement.Infrastructure.Tests.Mocks;
 using NetPayAdvance.LoanManagement.Persistence.Abstractions;
 using NetPayAdvance.LoanManagement.Persistence.Database;
 using Xunit;

 namespace NetPayAdvance.LoanManagement.Infrastructure.Tests.Services
 {
     public class BaseTestContext
     {
         protected DbContextOptions<LoanContext> options;

         public LoanContext DefaultContext => new LoanContext(options, new Mock<IDomainEventDispatcher>().Object,
             new Mock<ILogger<LoanContext>>().Object);

         public BaseTestContext()
         {
             //context = fixture.Context;
             options = new DbContextOptionsBuilder<LoanContext>()
                      .UseInMemoryDatabase(databaseName: "temp").Options;
         }
     }
     
     public class UnitTestSkipPaymentServices : BaseTestContext
     {
         private readonly IAdjustmentAggregateRepository agg;
         

         // [Fact]
         // public async Task VerifyBasicContract()
         // {
         //     var aggregate = new Mock<IAdjustmentAggregateRepository>();
         //     var facade = new Mock<IDbFacade>();
         //     var service = new SkipPaymentService(aggregate.Object, facade.Object);
         //     var loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(1000, 0.1m, 1m, LoanType.InterestBearing, 701),
         //         LoanStatus.Open, new Amount(500, 10, 10, 0, 0));
         //     typeof(Loan).GetProperty("LoanInfo").SetValue(loan, new LoanInfo(false), null);
         //     typeof(Loan).GetProperty("ProjectedPayments").SetValue(loan, new List<ProjectedPayment>()
         //     {
         //         new ProjectedPayment(1,new Amount(100,10,60,0,0),170,0,new Period(new DateOnly(2022,07,01),
         //             new DateOnly(2022,08,01)),new DateOnly(2022,08,01),false)
         //     }, null);
         //     aggregate.Setup(o => o.GetByIdAsync(It.IsAny<int>(), CancellationToken.None)).ReturnsAsync(
         //         new AdjustmentAggregate(loan, new List<Statement>()
         //         {
         //             new(new StatementId(1, new DateOnly(2022, 1, 4)), Amount.Zero.AddPrincipal(90).AddInterest(10), Amount.Zero.AddPrincipal(90).AddInterest(10),
         //                 new Period(new DateOnly(2021, 12, 4), new DateOnly(2022, 1, 4)), new DateOnly(2022, 1, 18))
         //         }));
         //     facade.Setup(
         //         o => o.QueryFirstOrDefaultAsync<DateOnly>(
         //             @"SELECT PayDate FROM PaydayFlex.dbo.FN_Paydates(@PowerID, @Paydates, @StartDate)",
         //         It.IsAny<Object>(),It.IsAny<IDbTransaction>())).ReturnsAsync(
         //         new DateOnly(2022, 08, 25)
         //     );
         //
         //     var statements = new List<Statement>()
         //     {
         //         new(new StatementId(1, new DateOnly(2022, 08, 01)), new Amount(100, 0, 0, 0, 0),
         //             new Amount(50, 1, 15, 0, 0),
         //             new Period(DateOnly.MinValue.AddYears(2000), DateOnly.MinValue.AddMonths(1).AddYears(2000)),
         //             DateOnly.MaxValue)
         //     };
         //     DefaultContext.AdjustmentAggregates.Add(
         //         new AdjustmentAggregate(loan, statements));
         //
         //
         //     var sb = await service.HtmlPayablesList(loan, new DateOnly(2022, 08, 01), statements);
         //     var output = "<table style=\"display:flex;justify-content:center\"><tr><td style=\"text-align:left;padding:4pt;font-size:10pt;\"> Payment 1 Due: $170.00</td><td style=\"text-align:left;padding:4pt;font-size:10pt;\"> Amt Paid: <strong>SKIPPED BY AGREEMENT</strong></td><td style=\"text-align:left;padding:4pt;font-size:10pt;\"> on: 7/29/2022</td></tr><td style=\"text-align:left;padding:4pt;font-size:10pt;\"> Payment 2 Due: $170.00</td><td style=\"text-align:left;padding:4pt;font-size:10pt;\"> Amt Paid: $0.00</td><td style=\"text-align:left;padding:4pt;font-size:10pt;\"> on: 8/25/2022</td></table>";
         //     Assert.Equal(output,sb);
         // }

     }
 }
