namespace Stripe
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Stripe.Infrastructure;

    public abstract class Service<EntityReturned>
        where EntityReturned : IStripeEntity
    {
        private IStripeClient stripeClient;

        protected Service()
        {
        }

        protected Service(string apiKey)
        {
            this.ApiKey = apiKey;
        }

        public string ApiKey { get; set; }

        public abstract string BasePath { get; }

        public virtual string BaseUrl => this.Client.ApiBase;

        /// <summary>
        /// Gets or sets the client used by this service to send requests. If <c>null</c>, then the
        /// default client in <see cref="StripeConfiguration.StripeClient"/> is used instead.
        /// </summary>
        public IStripeClient Client
        {
            get => this.stripeClient ?? StripeConfiguration.StripeClient;
            set => this.stripeClient = value;
        }

        protected EntityReturned CreateEntity(BaseOptions options, RequestOptions requestOptions)
        {
            return this.Request(
                HttpMethod.Post,
                this.ClassUrl(),
                options,
                requestOptions);
        }

        protected Task<EntityReturned> CreateEntityAsync(
            BaseOptions options,
            RequestOptions requestOptions,
            CancellationToken cancellationToken)
        {
            return this.RequestAsync(
                HttpMethod.Post,
                this.ClassUrl(),
                options,
                requestOptions,
                cancellationToken);
        }

        protected EntityReturned DeleteEntity(
            string id,
            BaseOptions options,
            RequestOptions requestOptions)
        {
            return this.Request(
                HttpMethod.Delete,
                this.InstanceUrl(id),
                options,
                requestOptions);
        }

        protected Task<EntityReturned> DeleteEntityAsync(
            string id,
            BaseOptions options,
            RequestOptions requestOptions,
            CancellationToken cancellationToken)
        {
            return this.RequestAsync(
                HttpMethod.Delete,
                this.InstanceUrl(id),
                options,
                requestOptions,
                cancellationToken);
        }

        protected EntityReturned GetEntity(
            string id,
            BaseOptions options,
            RequestOptions requestOptions)
        {
            return this.Request(
                HttpMethod.Get,
                this.InstanceUrl(id),
                options,
                requestOptions);
        }

        protected Task<EntityReturned> GetEntityAsync(
            string id,
            BaseOptions options,
            RequestOptions requestOptions,
            CancellationToken cancellationToken)
        {
            return this.RequestAsync(
                HttpMethod.Get,
                this.InstanceUrl(id),
                options,
                requestOptions,
                cancellationToken);
        }

        protected StripeList<EntityReturned> ListEntities(
            ListOptions options,
            RequestOptions requestOptions)
        {
            return this.Request<StripeList<EntityReturned>>(
                HttpMethod.Get,
                this.ClassUrl(),
                options,
                requestOptions);
        }

        protected Task<StripeList<EntityReturned>> ListEntitiesAsync(
            ListOptions options,
            RequestOptions requestOptions,
            CancellationToken cancellationToken)
        {
            return this.RequestAsync<StripeList<EntityReturned>>(
                HttpMethod.Get,
                this.ClassUrl(),
                options,
                requestOptions,
                cancellationToken);
        }

        protected IEnumerable<EntityReturned> ListEntitiesAutoPaging(
            ListOptions options,
            RequestOptions requestOptions)
        {
            return this.ListRequestAutoPaging<EntityReturned>(
                this.ClassUrl(),
                options,
                requestOptions);
        }

        protected EntityReturned UpdateEntity(
            string id,
            BaseOptions options,
            RequestOptions requestOptions)
        {
            return this.Request(
                HttpMethod.Post,
                this.InstanceUrl(id),
                options,
                requestOptions);
        }

        protected Task<EntityReturned> UpdateEntityAsync(
            string id,
            BaseOptions options,
            RequestOptions requestOptions,
            CancellationToken cancellationToken)
        {
            return this.RequestAsync(
                HttpMethod.Post,
                this.InstanceUrl(id),
                options,
                requestOptions,
                cancellationToken);
        }

        protected EntityReturned Request(
            HttpMethod method,
            string path,
            BaseOptions options,
            RequestOptions requestOptions)
        {
            return this.Request<EntityReturned>(
                method,
                path,
                options,
                requestOptions);
        }

        protected Task<EntityReturned> RequestAsync(
            HttpMethod method,
            string path,
            BaseOptions options,
            RequestOptions requestOptions,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.RequestAsync<EntityReturned>(
                method,
                path,
                options,
                requestOptions,
                cancellationToken);
        }

        protected T Request<T>(
            HttpMethod method,
            string path,
            BaseOptions options,
            RequestOptions requestOptions)
            where T : IStripeEntity
        {
            return this.RequestAsync<T>(method, path, options, requestOptions)
                .ConfigureAwait(false).GetAwaiter().GetResult();
        }

        protected async Task<T> RequestAsync<T>(
            HttpMethod method,
            string path,
            BaseOptions options,
            RequestOptions requestOptions,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : IStripeEntity
        {
            options = this.SetupOptions(options, IsStripeList<T>());
            requestOptions = this.SetupRequestOptions(requestOptions);
            return await this.Client.RequestAsync<T>(
                method,
                path,
                options,
                requestOptions,
                cancellationToken);
        }

        protected IEnumerable<T> ListRequestAutoPaging<T>(
            string url,
            ListOptions options,
            RequestOptions requestOptions)
            where T : IStripeEntity
        {
            var page = this.Request<StripeList<T>>(
                HttpMethod.Get,
                url,
                options,
                requestOptions);

            while (true)
            {
                string itemId = null;
                foreach (var item in page)
                {
                    itemId = ((IHasId)item).Id;
                    yield return item;
                }

                if (!page.HasMore || string.IsNullOrEmpty(itemId))
                {
                    break;
                }

                options.StartingAfter = itemId;
                page = this.Request<StripeList<T>>(
                    HttpMethod.Get,
                    url,
                    options,
                    requestOptions);
            }
        }

        protected BaseOptions SetupOptions(BaseOptions options, bool isListMethod)
        {
            var serviceExpansions = this.Expansions(isListMethod);
            if (!serviceExpansions.Any())
            {
                return options;
            }

            options = options ?? new BaseOptions();
            foreach (var expansion in serviceExpansions)
            {
                options.AddExpand(expansion);
            }

            return options;
        }

        protected RequestOptions SetupRequestOptions(RequestOptions requestOptions)
        {
            if (requestOptions == null)
            {
                requestOptions = new RequestOptions();
            }

            if (string.IsNullOrEmpty(requestOptions.ApiKey) && !string.IsNullOrEmpty(this.ApiKey))
            {
                requestOptions.ApiKey = this.ApiKey;
            }

            requestOptions.BaseUrl = requestOptions.BaseUrl ?? this.BaseUrl;

            return requestOptions;
        }

        protected virtual string ClassUrl()
        {
            return this.BasePath;
        }

        protected virtual string InstanceUrl(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException(
                    "The resource ID cannot be null or whitespace.",
                    nameof(id));
            }

            return $"{this.ClassUrl()}/{WebUtility.UrlEncode(id)}";
        }

        private static bool IsStripeList<T>()
        {
            var typeInfo = typeof(T).GetTypeInfo();
            return typeInfo.IsGenericType
                && typeInfo.GetGenericTypeDefinition() == typeof(StripeList<>);
        }

        /// <summary>
        /// Returns the list of attributes to expand in requests sent by the service.
        /// </summary>
        /// <param name="isListMethod">Whether the request is a list request or not.</param>
        /// <returns>The list of attributes to expand.</returns>
        public List<string> Expansions(bool isListMethod)
        {
            return this.GetType()
                .GetRuntimeProperties()
                .Where(p => p.Name.StartsWith("Expand") && p.PropertyType == typeof(bool))
                .Where(p => (bool)p.GetValue(this, null))
                .Select(p => StringUtils.ToSnakeCase(p.Name.Substring("Expand".Length)))
                .Select(i => isListMethod ? $"data.{i}" : i)
                .ToList();
        }
    }
}
