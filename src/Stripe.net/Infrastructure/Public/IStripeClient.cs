namespace Stripe
{
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for a Stripe client.
    /// </summary>
    public interface IStripeClient
    {
        /// <summary>Gets or sets the base URL for Stripe's API.</summary>
        /// <value>The base URL for Stripe's API.</value>
        string ApiBase { get; }

        /// <summary>Gets or sets the API key.</summary>
        /// <value>The API key.</value>
        string ApiKey { get; }

        /// <summary>Gets or sets the client ID.</summary>
        /// <value>The client ID.</value>
        string ClientId { get; }

        /// <summary>Gets or sets the base URL for Stripe's OAuth API.</summary>
        /// <value>The base URL for Stripe's OAuth API.</value>
        string ConnectBase { get; }

        /// <summary>Gets or sets the base URL for Stripe's Files API.</summary>
        /// <value>The base URL for Stripe's Files API.</value>
        string FilesBase { get; }

        /// <summary>Sends a request to Stripe's API as an asynchronous operation.</summary>
        /// <typeparam name="T">Type of the Stripe entity returned by the API.</typeparam>
        /// <param name="method">The HTTP method.</param>
        /// <param name="path">The path of the request.</param>
        /// <param name="options">The parameters of the request.</param>
        /// <param name="requestOptions">The special modifiers of the request.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        /// <exception cref="StripeException">Thrown if the request fails.</exception>
        Task<T> RequestAsync<T>(
            HttpMethod method,
            string path,
            BaseOptions options,
            RequestOptions requestOptions,
            CancellationToken cancellationToken = default(CancellationToken))
            where T : IStripeEntity;
    }
}
