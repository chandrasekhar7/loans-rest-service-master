using System.Threading.Tasks;
using System.Data;
using System.Threading;
using Moq;
using Xunit;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;
using NetPayAdvance.LoanManagement.Application.Models.Loans;
using static NetPayAdvance.LoanManagement.Application.Queries.GetStatementAdjustmentsQuery;
using NetPayAdvance.LoanManagement.Application.Queries;
using NetPayAdvance.LoanManagement.Domain.Entity.Adjustments;
using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
using NetPayAdvance.LoanManagement.Domain.Entity.Statements;
using System;
using System.Collections.Generic;

namespace NetPayAdvance.LoanManagement.Application.Tests.Queries
{
    public class GetStatementAdjustmentsQueryTest
    {
        public List<StatementAdjustmentModel> GetStatementAdjustmentModel()
        {
            List < StatementAdjustmentModel > _StatementAdjustmentModelList= new List<StatementAdjustmentModel>();
            StatementAdjustment _StatementAdjustment = new StatementAdjustment(AdjustmentType.CorrectionDecrease, new StatementId(1, new DateOnly(2022, 1, 4)), Amount.Zero.AddPrincipal(-50),"SP2");

            StatementAdjustmentModel _StatementAdjustmentModel  = new StatementAdjustmentModel(_StatementAdjustment);
             _StatementAdjustmentModelList.Add(_StatementAdjustmentModel);
            return _StatementAdjustmentModelList;
        }

        [Fact]
        public async Task GetsGenericSecurityQuestions()
        {
            //Arrange
            var mockFacade = new Mock<IDbFacade>();
            mockFacade.Setup(m => m.QueryMultipleAsync<StatementAdjustmentModel>(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>()))
                      .ReturnsAsync(GetStatementAdjustmentModel());

            var query = new GetStatementAdjustmentsQueryHandler(mockFacade.Object);
            var request = new GetStatementAdjustmentsQuery() { LoanID = 1 };

            // Act
            var result = await query.Handle(request, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Count > 0);
        }
    }
}
