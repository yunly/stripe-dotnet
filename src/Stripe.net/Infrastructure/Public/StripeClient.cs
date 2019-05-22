namespace Stripe
{
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// A Stripe client, used to issue requests to Stripe's API and deserialize responses.
    /// </summary>
    public class StripeClient : IStripeClient
    {
        private string apiKey;

        private string clientId;

        /// <summary>Initializes a new instance of the <see cref="StripeClient"/> class.</summary>
        /// <param name="apiKey">The API key to use to authenticate requests with Stripe.</param>
        /// <param name="clientId">The client ID to use in OAuth requests.</param>
        /// <param name="httpClient">
        /// The <see cref="IHttpClient"/> client to use. If <c>null</c>, an HTTP client will be
        /// created with default parameters.
        /// </param>
        public StripeClient(
            string apiKey,
            string clientId = null,
            IHttpClient httpClient = null)
        {
            this.ApiKey = apiKey;
            this.ClientId = clientId;
            this.HttpClient = httpClient ?? BuildDefaultHttpClient();
        }

        /// <summary>Default base URL for Stripe's API.</summary>
        public static string DefaultApiBase => "https://api.stripe.com";

        /// <summary>Default base URL for Stripe's OAuth API.</summary>
        public static string DefaultConnectBase => "https://connect.stripe.com";

        /// <summary>Default base URL for Stripe's Files API.</summary>
        public static string DefaultFilesBase => "https://files.stripe.com";

        /// <summary>Gets or sets the base URL for Stripe's API.</summary>
        /// <value>The base URL for Stripe's API.</value>
        public string ApiBase { get; set; } = DefaultApiBase;

#if NET45 || NETSTANDARD2_0
        /// <summary>Gets or sets the API key.</summary>
        /// <value>The API key.</value>
        /// <remarks>
        /// You can also set the API key using the <c>StripeApiKey</c> key in
        /// <see cref="System.Configuration.ConfigurationManager.AppSettings"/>.
        /// </remarks>
#else
        /// <summary>Gets or sets the API key.</summary>
        /// <value>The API key.</value>
#endif
        public string ApiKey
        {
            get
            {
#if NET45 || NETSTANDARD2_0
                if (string.IsNullOrEmpty(this.apiKey))
                {
                    this.apiKey = System.Configuration.ConfigurationManager.AppSettings["StripeApiKey"];
                }
#endif
                return this.apiKey;
            }

            set => this.apiKey = value;
        }

#if NET45 || NETSTANDARD2_0
        /// <summary>Gets or sets the client ID.</summary>
        /// <value>The client ID.</value>
        /// <remarks>
        /// You can also set the client ID using the <c>StripeClientId</c> key in
        /// <see cref="System.Configuration.ConfigurationManager.AppSettings"/>.
        /// </remarks>
#else
        /// <summary>Gets or sets the client ID.</summary>
        /// <value>The client ID.</value>
#endif
        public string ClientId
        {
            get
            {
#if NET45 || NETSTANDARD2_0
                if (string.IsNullOrEmpty(this.clientId))
                {
                    this.clientId = System.Configuration.ConfigurationManager.AppSettings["StripeClientId"];
                }
#endif
                return this.clientId;
            }

            set => this.clientId = value;
        }

        /// <summary>Gets or sets the base URL for Stripe's OAuth API.</summary>
        /// <value>The base URL for Stripe's OAuth API.</value>
        public string ConnectBase { get; set; } = DefaultConnectBase;

        /// <summary>Gets or sets the base URL for Stripe's Files API.</summary>
        /// <value>The base URL for Stripe's Files API.</value>
        public string FilesBase { get; set; } = DefaultFilesBase;

        /// <summary>Gets the <see cref="IHttpClient"/> used to send HTTP requests.</summary>
        /// <value>The <see cref="IHttpClient"/> used to send HTTP requests.</value>
        public IHttpClient HttpClient { get; internal set; }

        /// <summary>Sends a request to Stripe's API as an asynchronous operation.</summary>
        /// <typeparam name="T">Type of the Stripe entity returned by the API.</typeparam>
        /// <param name="method">The HTTP method.</param>
        /// <param name="path">The path of the request.</param>
        /// <param name="options">The parameters of the request.</param>
        /// <param name="requestOptions">The special modifiers of the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="StripeException">Thrown if the request fails.</exception>
        public async Task<T> RequestAsync<T>(
            HttpMethod method,
            string path,
            BaseOptions options,
            RequestOptions requestOptions,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : IStripeEntity
        {
            var request = new StripeRequest(this, method, path, options, requestOptions);

            var response = await this.HttpClient.MakeRequestAsync(request, cancellationToken);

            return ProcessResponse<T>(response);
        }

        private static IHttpClient BuildDefaultHttpClient()
        {
            var httpClient = new SystemNetHttpClient();
            httpClient.MaxNetworkRetries = StripeConfiguration.MaxNetworkRetries;
            return httpClient;
        }

        private static T ProcessResponse<T>(StripeResponse response)
            where T : IStripeEntity
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw BuildStripeException(response);
            }

            T obj;
            try
            {
                obj = StripeEntity.FromJson<T>(response.Content);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                throw BuildInvalidResponseException(response);
            }

            obj.StripeResponse = response;

            return obj;
        }

        private static StripeException BuildStripeException(StripeResponse response)
        {
            JObject jObject = null;

            try
            {
                jObject = JObject.Parse(response.Content);
            }
            catch (Newtonsoft.Json.JsonException)
            {
                return BuildInvalidResponseException(response);
            }

            // If the value of the `error` key is a string, then the error is an OAuth error
            // and we instantiate the StripeError object with the entire JSON.
            // Otherwise, it's a regular API error and we instantiate the StripeError object
            // with just the nested hash contained in the `error` key.
            var errorToken = jObject["error"];
            if (errorToken == null)
            {
                return BuildInvalidResponseException(response);
            }

            var stripeError = errorToken.Type == JTokenType.String
                ? StripeError.FromJson(response.Content)
                : StripeError.FromJson(errorToken.ToString());

            stripeError.StripeResponse = response;

            return new StripeException(
                response.StatusCode,
                stripeError,
                stripeError.Message ?? stripeError.ErrorDescription)
            {
                StripeResponse = response,
            };
        }

        private static StripeException BuildInvalidResponseException(StripeResponse response)
        {
            return new StripeException(
                response.StatusCode,
                null,
                $"Invalid response object from API: \"{response.Content}\"")
            {
                StripeResponse = response,
            };
        }
    }
}
