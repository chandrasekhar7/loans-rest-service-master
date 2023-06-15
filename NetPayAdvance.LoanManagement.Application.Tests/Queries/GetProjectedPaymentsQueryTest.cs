
using System;
using Moq;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using Xunit;
using System.Linq;
using NetPayAdvance.LoanManagement.Domain.Entity;
using NetPayAdvance.LoanManagement.Persistence.Tests.Fixtures;
using System.Globalization;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;
using static NetPayAdvance.LoanManagement.Application.Queries.GetProjectedPaymentsQuery;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using System.Threading;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Application.Queries;

namespace NetPayAdvance.LoanManagement.Application.Tests.Queries
{

    public class GetProjectedPaymentsQueryTest : BaseTestContext
    {
        private Mock<ILoanRepository> repo;

        public GetProjectedPaymentsQueryTest()
        {
            repo = new Mock<ILoanRepository>();
        }
        public Loan ProjectedPayments()
        {
            DateOnly date = new(2011, 02, 19);
            var ProjectedPayment = new ProjectedPayment(1, Amount.Zero.AddPrincipal(-50), 1, 1, null, date, false);
            var loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702),
            LoanStatus.Open, new Amount(1, 0, 0, 0, 0));
            loan.ProjectedPayments.Add(ProjectedPayment);
            return loan;
        }

        [Fact]
        public async void GetByIdAsync()
        {
            //Arrange
            DateOnly date = new(2011, 02, 19);
            var mockFacade = new Mock<IDbFacade>();
            DateOnly date1 = new(2011, 02, 19);
            var ProjectedPayment = new ProjectedPayment(1, Amount.Zero.AddPrincipal(-50), 1, 1, null, date1, false);
            var PaymentArrangement = new GetProjectedPaymentsQuery()
            {
                LoanID = 1

            };
            repo.Setup(m => m.GetByIdAsync(ProjectedPayment.LoanID, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ProjectedPayments());
            var query = new GetProjectedPaymentsQueryHandler(repo.Object);

            // Act
            var result = query.Handle(PaymentArrangement, CancellationToken.None);

            // Assert
             Assert.True(result.Result.Count >0);
        }

    }
}

