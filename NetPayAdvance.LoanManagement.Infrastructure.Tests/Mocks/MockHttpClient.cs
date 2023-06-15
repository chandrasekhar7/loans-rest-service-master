using System.Net.Http;
using Moq;

namespace NetPayAdvance.LoanManagement.Infrastructure.Tests.Mocks
{
    public class MockHttpClient
    {
        public HttpClient Client { get; }
        public Mock<HttpMessageHandler> Mock { get; }

        public MockHttpClient(HttpClient client, Mock<HttpMessageHandler> handler)
        {
            Client = client;
            Mock = handler;
        }
    }
}