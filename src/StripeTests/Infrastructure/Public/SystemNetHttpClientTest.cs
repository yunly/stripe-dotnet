namespace StripeTests
{
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;
    using Moq;
    using Moq.Protected;
    using Newtonsoft.Json.Linq;
    using Stripe;
    using Xunit;

    public class SystemNetHttpClientTest : BaseStripeTest
    {
        [Fact]
        public async Task MakeRequestAsync()
        {
            var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
            responseMessage.Content = new StringContent("Hello world!");
            var mockHandler = new Mock<HttpClientHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .Returns(Task.FromResult(responseMessage));
            var client = new SystemNetHttpClient(new HttpClient(mockHandler.Object));
            var options = new RequestOptions { ApiKey = "sk_test" };
            var request = new StripeRequest(new StripeClient("sk_test_123"), HttpMethod.Post, "/foo", null, options);

            var response = await client.MakeRequestAsync(request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("Hello world!", response.Content);
        }

        [Fact]
        public async Task UserAgentIncludesAppInfo()
        {
            var origAppInfo = StripeConfiguration.AppInfo;

            try
            {
                StripeConfiguration.AppInfo = new AppInfo
                {
                    Name = "MyAwesomeApp",
                    PartnerId = "pp_123",
                    Version = "1.2.34",
                    Url = "https://myawesomeapp.info"
                };

                var responseMessage = new HttpResponseMessage(HttpStatusCode.OK);
                responseMessage.Content = new StringContent("Hello world!");
                var mockHandler = new Mock<HttpClientHandler>();
                mockHandler.Protected()
                    .Setup<Task<HttpResponseMessage>>(
                        "SendAsync",
                        ItExpr.IsAny<HttpRequestMessage>(),
                        ItExpr.IsAny<CancellationToken>())
                    .Returns(Task.FromResult(responseMessage));

                var client = new SystemNetHttpClient(new System.Net.Http.HttpClient(mockHandler.Object));
                var options = new RequestOptions { ApiKey = "sk_test" };
                var request = new StripeRequest(new StripeClient("sk_test_123"), HttpMethod.Post, "/foo", null, options);
                await client.MakeRequestAsync(request);

                mockHandler.Protected()
                    .Verify(
                        "SendAsync",
                        Times.Once(),
                        ItExpr.Is<HttpRequestMessage>(m => this.VerifyHeaders(m.Headers)),
                        ItExpr.IsAny<CancellationToken>());
            }
            finally
            {
                StripeConfiguration.AppInfo = origAppInfo;
            }
        }

        private bool VerifyHeaders(HttpRequestHeaders headers)
        {
            var userAgent = headers.UserAgent.ToString();
            var appInfo = JObject.Parse(headers.GetValues("X-Stripe-Client-User-Agent").First())["application"];

            return userAgent.Contains("MyAwesomeApp/1.2.34 (https://myawesomeapp.info)") &&
                appInfo.Value<string>("name") == "MyAwesomeApp" &&
                appInfo.Value<string>("partner_id") == "pp_123" &&
                appInfo.Value<string>("version") == "1.2.34" &&
                appInfo.Value<string>("url") == "https://myawesomeapp.info";
        }
    }
}
