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
    
    public class AdjustmentCodeTest : BaseTestContext
    {

        public async Task<LoanAdjustment?> GetStatementModel()
        {
            var adj = new LoanAdjustment(1, "ZZZ", new Adjustment(Debit.Payment, Amount.Zero.AddPrincipal(-50)), new List<StatementAdjustment>(), 1);

            AdjustmentModel _AdjustmentModel = new AdjustmentModel(adj);
            return adj;
        }

        [Fact]
        public async void GetAdjustmentByID()
        {
            //Arrange
            var mockFacade = new Mock<IAdjustmentRepository>();
            var adj = new GetAdjustmentQuery(1)
            {
                AdjustmentId = 1

            };
            mockFacade
                .Setup(x => x.GetByIdAsync(adj.AdjustmentId, It.IsAny<CancellationToken>())
                ).Returns(GetStatementModel());

            var query = new GetAdjustmentQueryHandler(mockFacade.Object);
       

            // Act
            var result = await query.Handle(new GetAdjustmentQuery(1), CancellationToken.None);

            // Assert
            Assert.NotNull(result);
        }
    }
}
