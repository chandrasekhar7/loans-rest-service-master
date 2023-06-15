using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace NetPayAdvance.LoanManagement.Infrastructure.Tests.Mocks
{
    public class MockHttpClientFactory
    {
        public MockHttpClient Create(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(response);

            var client = new HttpClient(handlerMock.Object);
            
            // Base address must be set even though we arent going to use it
            client.BaseAddress = new Uri("http://localhost");
            
            return new MockHttpClient(client, handlerMock);
        }

        public MockHttpClient Create(List<HttpResponseMessage> responses)
        {
            var handlerMock = new Mock<HttpMessageHandler>();
            var part = handlerMock
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
            
            responses.ForEach(r => part.ReturnsAsync(r));

            var client = new HttpClient(handlerMock.Object);
            
            // Base address must be set even though we arent going to use it
            client.BaseAddress = new Uri("http://localhost");
            
            return new MockHttpClient(client, handlerMock);
        }
    }
}