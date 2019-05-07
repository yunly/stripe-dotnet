namespace Stripe
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Newtonsoft.Json;
    using Stripe.Infrastructure;

    /// <summary>
    /// Global configuration class for Stripe.net settings.
    /// </summary>
    public static class StripeConfiguration
    {
        private static string apiKey;

        private static AppInfo appInfo;

        private static string clientId;

        private static IStripeClient stripeClient;

        static StripeConfiguration()
        {
            StripeNetVersion = new AssemblyName(typeof(StripeConfiguration).GetTypeInfo().Assembly.FullName).Version.ToString(3);
        }

        /// <summary>API version used by Stripe.net.</summary>
        public static string ApiVersion => "2019-03-14";

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
        public static string ApiKey
        {
            get => StripeClient.ApiKey;

            set
            {
                if (value != apiKey)
                {
                    StripeClient = null;
                }

                apiKey = value;
            }
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
        public static string ClientId
        {
            get => StripeClient.ClientId;

            set
            {
                if (value != clientId)
                {
                    StripeClient = null;
                }

                clientId = value;
            }
        }

        /// <summary>
        /// Gets or sets the maximum number of times that the library will retry requests that
        /// appear to have failed due to an intermittent problem.
        /// </summary>
        /// <remarks>
        /// This global setting is only used when the <see cref="SystemNetHttpClient"/> instance is
        /// created automatically. If you're manually providing your own client (e.g. to use a proxy
        /// or a custom message handler), you'll need to set
        /// <see cref="SystemNetHttpClient.MaxNetworkRetries"/> directly.
        /// </remarks>
        public static int MaxNetworkRetries { get; set; }

        /// <summary>
        /// Gets or sets the settings used for deserializing JSON objects returned by Stripe's API.
        /// It is highly recommended you do not change these settings, as doing so can produce
        /// unexpected results. If you do change these settings, make sure that
        /// <see cref="Stripe.Infrastructure.StripeObjectConverter"/> is among the converters,
        /// otherwise Stripe.net will no longer be able to deserialize polymorphic resources
        /// represented by interfaces (e.g. <see cref="IPaymentSource"/>).
        /// </summary>
        public static JsonSerializerSettings SerializerSettings { get; set; } = DefaultSerializerSettings();

        /// <summary>
        /// Gets or sets a custom <see cref="StripeClient"/> for sending requests to Stripe's
        /// API. You can use this to use a custom message handler, set proxy parameters, etc.
        /// </summary>
        /// <example>
        /// To use a custom message handler:
        /// <code>
        /// System.Net.Http.HttpMessageHandler messageHandler = ...;
        /// var httpClient = new System.Net.HttpClient(messageHandler);
        /// var stripeClient = new Stripe.StripeClient(
        ///     apiKey,
        ///     httpClient: new Stripe.SystemNetHttpClient(httpClient));
        /// Stripe.StripeConfiguration.StripeClient = stripeClient;
        /// </code>
        /// </example>
        public static IStripeClient StripeClient
        {
            get
            {
                if (stripeClient == null)
                {
                    stripeClient = new StripeClient(apiKey, clientId);
                }

                return stripeClient;
            }

            set => stripeClient = value;
        }

        /// <summary>Gets the version of the Stripe.net client library.</summary>
        public static string StripeNetVersion { get; }

        /// <summary>
        /// Sets information about the "app" which this integration belongs to. This should be
        /// reserved for plugins that wish to identify themselves with Stripe.
        /// </summary>
        public static AppInfo AppInfo
        {
            internal get => appInfo;

            set
            {
                if ((value != null) && string.IsNullOrEmpty(value.Name))
                {
                    throw new ArgumentException("AppInfo.Name cannot be empty");
                }

                appInfo = value;

                // This is run when the client is first initialized, but we need to reinitialize
                // now that we have some app info.
                // This is done through ugly casting because we don't want to make this part of
                // the IStripeClient and IHTTP client interfaces.
                ((StripeClient as StripeClient)?.HttpClient as SystemNetHttpClient)?.InitUserAgentStrings();
            }
        }

        /// <summary>
        /// Returns a new instance of <see cref="Newtonsoft.Json.JsonSerializerSettings"/> with
        /// the default settings used by Stripe.net.
        /// </summary>
        /// <returns>A <see cref="Newtonsoft.Json.JsonSerializerSettings"/> instance.</returns>
        public static JsonSerializerSettings DefaultSerializerSettings()
        {
            return new JsonSerializerSettings
            {
                Converters = new List<JsonConverter>
                {
                    new StripeObjectConverter(),
                },
                DateParseHandling = DateParseHandling.None,
            };
        }

        // TODO: remove everything below this in a future major version

        /// <summary>
        /// Sets the API key.
        /// This method is deprecated and will be removed in a future version, please use the
        /// <see cref="ApiKey"/> property setter instead.
        /// </summary>
        /// <param name="newApiKey">API key.</param>
        [Obsolete("Use StripeConfiguration.ApiKey setter instead.")]
        public static void SetApiKey(string newApiKey)
        {
            ApiKey = newApiKey;
        }
    }
}
