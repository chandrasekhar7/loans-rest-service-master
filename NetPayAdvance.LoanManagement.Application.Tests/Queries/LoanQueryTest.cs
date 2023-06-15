using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NetPayAdvance.LoanManagement.Application.Abstractions;
using NetPayAdvance.LoanManagement.Application.Commands.Adjustments;
using NetPayAdvance.LoanManagement.Application.Commands.Statements.UpdateStatement;
using NetPayAdvance.LoanManagement.Application.ErrorHandling;
using NetPayAdvance.LoanManagement.Application.Models.Common;
using NetPayAdvance.LoanManagement.Application.Models.Inputs;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using NetPayAdvance.LoanManagement.Application.Queries;
using NetPayAdvance.LoanManagement.Application.Services;
using NetPayAdvance.LoanManagement.Application.Users;
using NetPayAdvance.LoanManagement.Domain.Abstractions;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using Xunit;
using System.Linq;
using NetPayAdvance.LoanManagement.Domain.Entity;
using NetPayAdvance.LoanManagement.Persistence.Tests.Fixtures;

namespace NetPayAdvance.LoanManagement.Application.Tests.Queries
{
    public class LoanQueryTest : BaseTestContext
    {

        public async Task<Loan?> GetloanModel()
        {
            var loan = new Loan(1, DateTime.Now, null, null, new LoanConfig(500m, 0.1m, 1m, LoanType.InterestBearing, 702), LoanStatus.Open, new Amount(1, 0, 0, 0, 0));
            await using var context = DefaultContext;
            await context.Loans.AddAsync(loan);
            await context.LoanPermissions.AddAsync(new LoanPermissions { LoanId = 1, AutoDebit = false, AutoACH = false });
            await context.LoanContracts.AddAsync(new LoanContracts() {LoanId = 1, DateSigned = DateTime.Now.AddDays(-15)});
            await context.SaveChangesAsync();
            var data = context.Loans.FirstOrDefault(w => w.LoanId == 1);
            return data;
        }

        [Fact]
        public async void GetByIdAsync()
        {
            //Arrange
            var mockFacade = new Mock<ILoanRepository>();
            var adj = new GetLoanQuery(1)
            {
                LoanId = 1
            };
            

            mockFacade
                .Setup(x => x.GetByIdAsync(adj.LoanId, It.IsAny<CancellationToken>())
                ).Returns(GetloanModel());

            var query = new GetLoanQueryHandler(mockFacade.Object);
            var PaymentArrangement = new GetPaymentArrangementQuery()
            {
                LoanID = 1

            };

            // Act
            var result = await query.Handle(new GetLoanQuery(adj.LoanId), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
        }
    }
}

