//using NetPayAdvance.Common.Enums;
//using NetPayAdvance.LoanManagement.Domain.Entity.Loans;
//using NetPayAdvance.LoanManagement.Application.ViewModels;
//using NetPayAdvance.LoanManagement.Infrastructure.Abstractions;
//using NetPayAdvance.LoanManagement.Infrastructure.Common;
//using NetPayAdvance.LoanManagement.Infrastructure.Services;
using NetPayAdvance.Transactions.Tests.Helpers;
using NetPayAdvance.LoanManagement.Infrastructure.Tests.Mocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace NetPayAdvance.LoanManagement.Infrastructure.Tests.Services
{
    public class UnitTestPaymentServices
    {
    //    SkipPaymentService _SkipPaymentService = null;
    //     IContractService _contract = null;
    //    List<DateTime> skipDates = new List<DateTime>();
    //    List<DateTime> nextPaydates = new List<DateTime>();

    //    [Fact]
    //    public async Task VerifyRunTokenizedTransaction()
    //    {
    //        var res = new HttpResponseMessage(HttpStatusCode.OK);
    //        res.Content = new StringContent(FileHelper.LoadString("CardResponseViewModelResponse.json"));
    //        var mockClient = new MockHttpClientFactory().Create(res);
    //        var service = new PaymentService(mockClient.Client);
    //        CardRequestViewModel _CardResponseViewModel =  new CardRequestViewModel();
    //        _CardResponseViewModel.LoanID = 123;
    //        _CardResponseViewModel.PowerID = 321;
    //        _CardResponseViewModel.Amount = (decimal)23.00;
    //        _CardResponseViewModel.CardID = 009900;

    //        var response1 = await service.RunTokenizedTransaction(_CardResponseViewModel);
    //        response1.LoanId.Equals(123);
    //        response1.Amount.Equals((decimal)23.00);
    //    }

    //    [Fact]
    //    public async Task VerifyRunTransaction()
    //    {
    //        var res = new HttpResponseMessage(HttpStatusCode.OK);
    //        res.Content = new StringContent(FileHelper.LoadString("CardResponseViewModelResponse.json"));
    //        var mockClient = new MockHttpClientFactory().Create(res);
    //        var service = new PaymentService(mockClient.Client);
    //        CardRequestViewModel _CardResponseViewModel = new CardRequestViewModel();
    //        _CardResponseViewModel.LoanID = 123;
    //        _CardResponseViewModel.PowerID = 321;
    //        _CardResponseViewModel.Amount = (decimal)23.00;
    //        _CardResponseViewModel.CardID = 009900;

    //        var response1 = await service.RunTokenizedTransaction(_CardResponseViewModel);
    //        response1.LoanId.Equals(123);
    //        response1.Amount.Equals((decimal)23.00);
    //    }

    //    [Fact]
    //    public async Task VerifyFund()
    //    {
    //        var res = new HttpResponseMessage(HttpStatusCode.OK);
    //        res.Content = new StringContent(FileHelper.LoadString("CardResponseViewModelResponse.json"));
    //        var mockClient = new MockHttpClientFactory().Create(res);
    //        var service = new PaymentService(mockClient.Client);
    //        CardRequestViewModel _CardResponseViewModel = new CardRequestViewModel();
    //        _CardResponseViewModel.LoanID = 123;
    //        _CardResponseViewModel.PowerID = 321;
    //        _CardResponseViewModel.Amount = (decimal)23.00;
    //        _CardResponseViewModel.CardID = 009900;

    //        var response1 = await service.RunTokenizedTransaction(_CardResponseViewModel);
    //        response1.LoanId.Equals(123);
    //        response1.Amount.Equals((decimal)23.00);
    //    }
    }
}
