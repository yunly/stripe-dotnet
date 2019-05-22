namespace Stripe
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Stripe.Infrastructure.FormEncoding;

    public class OAuthTokenService : Service<OAuthToken>,
        ICreatable<OAuthToken, OAuthTokenCreateOptions>
    {
        public OAuthTokenService()
            : base(null)
        {
        }

        public OAuthTokenService(string apiKey)
            : base(apiKey)
        {
        }

        public override string BasePath => "/oauth/token";

        public override string BaseUrl => this.Client.ConnectBase;

        public virtual Uri AuthorizeUrl(OAuthAuthorizeUrlOptions options, bool express = false)
        {
            if (options == null)
            {
                options = new OAuthAuthorizeUrlOptions();
            }

            if (string.IsNullOrEmpty(options.ClientId))
            {
                options.ClientId = this.Client.ClientId;
            }

            string path = "/oauth/authorize";
            if (express)
            {
                path = "/express" + path;
            }

            return new Uri(this.Client.ConnectBase + path + "?" +
                FormEncoder.CreateQueryString(options));
        }

        public virtual OAuthToken Create(OAuthTokenCreateOptions options, RequestOptions requestOptions = null)
        {
            if (options == null)
            {
                options = new OAuthTokenCreateOptions();
            }

            if (string.IsNullOrEmpty(options.ClientSecret))
            {
                options.ClientSecret = this.Client.ApiKey;
            }

            return this.CreateEntity(options, requestOptions);
        }

        public virtual Task<OAuthToken> CreateAsync(OAuthTokenCreateOptions options, RequestOptions requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (options == null)
            {
                options = new OAuthTokenCreateOptions();
            }

            if (string.IsNullOrEmpty(options.ClientSecret))
            {
                options.ClientSecret = this.Client.ApiKey;
            }

            return this.CreateEntityAsync(options, requestOptions, cancellationToken);
        }

        public virtual OAuthDeauthorize Deauthorize(OAuthDeauthorizeOptions options, RequestOptions requestOptions = null)
        {
            if (options == null)
            {
                options = new OAuthDeauthorizeOptions();
            }

            if (string.IsNullOrEmpty(options.ClientId))
            {
                options.ClientId = this.Client.ClientId;
            }

            return this.Request<OAuthDeauthorize>(HttpMethod.Post, "/oauth/deauthorize", options, requestOptions);
        }

        public virtual Task<OAuthDeauthorize> DeauthorizeAsync(OAuthDeauthorizeOptions options, RequestOptions requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (options == null)
            {
                options = new OAuthDeauthorizeOptions();
            }

            if (string.IsNullOrEmpty(options.ClientId))
            {
                options.ClientId = this.Client.ClientId;
            }

            return this.RequestAsync<OAuthDeauthorize>(HttpMethod.Post, "/oauth/deauthorize", options, requestOptions, cancellationToken);
        }
    }
}
