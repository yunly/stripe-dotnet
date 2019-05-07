namespace StripeTests
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Stripe;
    using StripeTests.Infrastructure.TestData;
    using Xunit;

    public class StripeRequestTest : BaseStripeTest
    {
        private IStripeClient client = new DummyStripeClient();

        [Fact]
        public void Ctor_GetRequest()
        {
            var options = new TestOptions { String = "string!" };
            var requestOptions = new RequestOptions();
            var request = new StripeRequest(this.client, HttpMethod.Get, "/get", options, requestOptions);

            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal($"{this.client.ApiBase}/get?string=string!", request.Uri.ToString());
            Assert.Equal($"Bearer {this.client.ApiKey}", request.AuthorizationHeader.ToString());
            Assert.True(request.StripeHeaders.ContainsKey("Stripe-Version"));
            Assert.Equal(StripeConfiguration.ApiVersion, request.StripeHeaders["Stripe-Version"]);
            Assert.False(request.StripeHeaders.ContainsKey("Idempotency-Key"));
            Assert.False(request.StripeHeaders.ContainsKey("Stripe-Account"));
            Assert.Null(request.Content);
        }

        [Fact]
        public async Task Ctor_PostRequest()
        {
            var options = new TestOptions { String = "string!" };
            var requestOptions = new RequestOptions();
            var request = new StripeRequest(this.client, HttpMethod.Post, "/post", options, requestOptions);

            Assert.Equal(HttpMethod.Post, request.Method);
            Assert.Equal($"{this.client.ApiBase}/post", request.Uri.ToString());
            Assert.Equal($"Bearer {this.client.ApiKey}", request.AuthorizationHeader.ToString());
            Assert.True(request.StripeHeaders.ContainsKey("Stripe-Version"));
            Assert.Equal(StripeConfiguration.ApiVersion, request.StripeHeaders["Stripe-Version"]);
            Assert.True(request.StripeHeaders.ContainsKey("Idempotency-Key"));
            Assert.False(request.StripeHeaders.ContainsKey("Stripe-Account"));
            Assert.NotNull(request.Content);
            var content = await request.Content.ReadAsStringAsync();
            Assert.Equal("string=string!", content);
        }

        [Fact]
        public void Ctor_DeleteRequest()
        {
            var options = new TestOptions { String = "string!" };
            var requestOptions = new RequestOptions();
            var request = new StripeRequest(this.client, HttpMethod.Delete, "/delete", options, requestOptions);

            Assert.Equal(HttpMethod.Delete, request.Method);
            Assert.Equal($"{this.client.ApiBase}/delete?string=string!", request.Uri.ToString());
            Assert.Equal($"Bearer {this.client.ApiKey}", request.AuthorizationHeader.ToString());
            Assert.True(request.StripeHeaders.ContainsKey("Stripe-Version"));
            Assert.Equal(StripeConfiguration.ApiVersion, request.StripeHeaders["Stripe-Version"]);
            Assert.False(request.StripeHeaders.ContainsKey("Idempotency-Key"));
            Assert.False(request.StripeHeaders.ContainsKey("Stripe-Account"));
            Assert.Null(request.Content);
        }

        [Fact]
        public void Ctor_RequestOptions()
        {
            var requestOptions = new RequestOptions
            {
                ApiKey = "sk_override",
                IdempotencyKey = "idempotency_key",
                StripeAccount = "acct_456",
                BaseUrl = "https://example.com",
                StripeVersion = "2012-12-21",
            };
            var request = new StripeRequest(this.client, HttpMethod.Get, "/get", null, requestOptions);

            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.Equal("https://example.com/get", request.Uri.ToString());
            Assert.Equal("Bearer sk_override", request.AuthorizationHeader.ToString());
            Assert.True(request.StripeHeaders.ContainsKey("Stripe-Version"));
            Assert.Equal("2012-12-21", request.StripeHeaders["Stripe-Version"]);
            Assert.True(request.StripeHeaders.ContainsKey("Idempotency-Key"));
            Assert.Equal("idempotency_key", request.StripeHeaders["Idempotency-Key"]);
            Assert.True(request.StripeHeaders.ContainsKey("Stripe-Account"));
            Assert.Equal("acct_456", request.StripeHeaders["Stripe-Account"]);
            Assert.Null(request.Content);
        }

        [Fact]
        public void Ctor_ThrowsIfApiKeyIsNull()
        {
            var client = new DummyStripeClient();
            client.ApiKey = null;

            var options = new TestOptions();
            var requestOptions = new RequestOptions();

            var exception = Assert.Throws<StripeException>(() =>
                new StripeRequest(client, HttpMethod.Get, "/get", options, requestOptions));

            Assert.Contains("No API key provided.", exception.Message);
        }

        [Fact]
        public void Ctor_ThrowsIfApiKeyIsEmpty()
        {
            var client = new DummyStripeClient();
            client.ApiKey = string.Empty;

            var options = new TestOptions();
            var requestOptions = new RequestOptions();

            var exception = Assert.Throws<StripeException>(() =>
                new StripeRequest(client, HttpMethod.Get, "/get", options, requestOptions));

            Assert.Contains("No API key provided.", exception.Message);
        }

        [Fact]
        public void Ctor_ThrowsIfApiKeyContainsWhitespace()
        {
            var client = new DummyStripeClient();
            client.ApiKey = "sk_test_123\n";

            var options = new TestOptions();
            var requestOptions = new RequestOptions();

            var exception = Assert.Throws<StripeException>(() =>
                new StripeRequest(client, HttpMethod.Get, "/get", options, requestOptions));

            Assert.Contains(
                "Your API key is invalid, as it contains whitespace.",
                exception.Message);
        }

        private class DummyStripeClient : IStripeClient
        {
            public string ApiBase { get; set; } = StripeClient.DefaultApiBase;

            public string ApiKey { get; set; } = "sk_dummy";

            public string ClientId { get; set; }

            public string ConnectBase { get; set; } = StripeClient.DefaultConnectBase;

            public string FilesBase { get; set; } = StripeClient.DefaultFilesBase;

            public Task<T> RequestAsync<T>(
                HttpMethod method,
                string path,
                BaseOptions options,
                RequestOptions requestOptions,
                CancellationToken cancellationToken = default(CancellationToken))
                where T : IStripeEntity
            {
                return null;
            }
        }
    }
}
