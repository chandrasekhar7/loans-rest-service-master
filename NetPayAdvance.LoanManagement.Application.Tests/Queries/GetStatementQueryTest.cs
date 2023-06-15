using System;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using Xunit;
using System.Linq;
using NetPayAdvance.LoanManagement.Persistence.Tests.Fixtures;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using System.Collections.Generic;
using NetPayAdvance.LoanManagement.Application.Queries;
using Moq;
using System.Threading;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;
using System.Data;
using System.Threading.Tasks;
using NetPayAdvance.LoanManagement.Domain.Abstractions;

namespace NetPayAdvance.LoanManagement.Application.Tests.Queries
{
    public class GetStatementQueryTest : BaseTestContext
    {
        public async Task<IReadOnlyList<Statement>> GetStatementModels()
        {
            List<Statement> _StatementModelList = new List<Statement>();
            var statement = new Statement(new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(16)), new Amount(100, 10, 50, 0, 0), new Amount(100, 10, 50, 0, 0),
               new Period(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now).AddDays(16)), DateOnly.FromDateTime(DateTime.Now).AddDays(30));
            var statement1 = new Statement(new StatementId(2, DateOnly.FromDateTime(DateTime.Now).AddDays(16)), new Amount(100, 10, 50, 0, 0), new Amount(100, 10, 50, 0, 0),
                                   new Period(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now).AddDays(16)), DateOnly.FromDateTime(DateTime.Now).AddDays(30));

            _StatementModelList.Add(statement);
            return _StatementModelList.AsReadOnly();
        }

        public async Task<Statement?> GetStatementModel()
        {
            var statement = new Statement(new StatementId(1, DateOnly.FromDateTime(DateTime.Now).AddDays(16)), new Amount(100, 10, 50, 0, 0), new Amount(100, 10, 50, 0, 0),
               new Period(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now).AddDays(16)), DateOnly.FromDateTime(DateTime.Now).AddDays(30));
            return statement;
        }

        [Fact]
        public async void GetByIdAsync()
        {
           //Arrange
            var mockFacade = new Mock<IStatementRepository>();
            var adj = new StatementId(1, new DateOnly(2022, 1, 4));

            mockFacade
                .Setup(x=>x.GetByIdAsync(adj, It.IsAny<CancellationToken>())
                ).Returns(GetStatementModel());

            var query = new GetStatementQueryHandler(mockFacade.Object);
            var PaymentArrangement = new GetPaymentArrangementQuery()
            {
                LoanID = 1

            };

            // Act
            var result = await query.Handle(new GetStatementQuery(adj), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
        }

        //[Fact]
        //public async void GetAsync()
        //{
        //    //Arrange
        //    var mockFacade = new Mock<IStatementRepository>();
        //    var abjnew = new GetStatementsQuery(1, true, new DateOnly(2022, 1, 4));
        //    var filter = new StatementFilter()
        //    {
        //        LoanId = 1,
        //        Balance = true,
        //        EndDate = new DateOnly(2022, 1, 4)
        //    };
        //    mockFacade
        //        .Setup(x => x.GetAsync(filter, It.IsAny<CancellationToken>())
        //        ).Returns(GetStatementModels());

        //    var query = new GetStatementsQueryHandler(mockFacade.Object);
        //    var PaymentArrangement = new GetPaymentArrangementQuery()
        //    {
        //        LoanID = 1

        //    };
        //    // Act
        //    var result = await query.Handle(abjnew, CancellationToken.None);

        //    // Assert
        //    Assert.NotNull(result);
        //}
    }
}

