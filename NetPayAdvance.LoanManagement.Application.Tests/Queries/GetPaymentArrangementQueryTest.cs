using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Xunit;
using System.Collections.Generic;
using static NetPayAdvance.LoanManagement.Application.Queries.GetPaymentArrangementQuery;
using NetPayAdvance.LoanManagement.Application.Queries;
using NetPayAdvance.LoanManagement.Application.Models.Common;
using NetPayAdvance.LoanManagement.Persistence.Abstractions;
using NetPayAdvance.LoanManagement.Persistence.Tests.Fixtures;
using System.Collections.ObjectModel;

namespace NetPayAdvance.LoanManagement.Application.Tests.Queries;

public class GetPaymentArrangementQueryTest : BaseTestContext
{

    public ReadOnlyCollection<PaymentArrangement> GetPaymentArrangementl()
    {
        List <PaymentArrangement> _PaymentArrangementList = new List<PaymentArrangement>();
        PaymentArrangement _PaymentArrangement = new PaymentArrangement();
        _PaymentArrangement.LoanID = 1;
        _PaymentArrangement.CreatedBy = "SP2";
        _PaymentArrangement.CreatedOn = System.DateTime.Now;
        _PaymentArrangement.PaymentDate = System.DateTime.Now;
        PaymentArrangement _PaymentArrangement1 = new PaymentArrangement();
        _PaymentArrangement1.LoanID = 2;
        _PaymentArrangement1.CreatedBy = "SP2";
        _PaymentArrangement1.CreatedOn = System.DateTime.Now;
        _PaymentArrangement1.PaymentDate = System.DateTime.Now;
        _PaymentArrangementList.Add(_PaymentArrangement);
        _PaymentArrangementList.Add(_PaymentArrangement1);
        //var readOnlyList = new IReadOnlyList<PaymentArrangement>(_PaymentArrangementList);
        return _PaymentArrangementList.AsReadOnly();
    }
    [Fact]
    public async Task GetCustomerInfoQuery()
    {
        
        //Arrange
       // var mockFacade = new Mock<IDbFacade>();
       // var adj = new PaymentArrangement()
       // {
       //     LoanID = 1
       // };
       // IReadOnlyList<PaymentArrangement> payments = GetPaymentArrangementl();
       // var mockList = new ReadOnlyCollection<PaymentArrangement>(GetPaymentArrangementl());
       // mockFacade
       //     .Setup(m => m.QueryAsync<PaymentArrangement>(It.IsAny<string>(), adj, It.IsAny<IDbTransaction>())
       //     ).ReturnsAsync(mockList);

       // var query = new GetPaymentArrangementQueryHandler(mockFacade.Object);
       // var PaymentArrangement = new GetPaymentArrangementQuery()
       // {
       //    LoanID =1
           
       // };

       // // Act
       // var result = await query.Handle(new GetPaymentArrangementQuery() { LoanID= 1 }, CancellationToken.None);

       // // Assert
       //Assert.NotNull(result);
     
    }


}