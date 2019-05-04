namespace Stripe
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class CustomerBalanceTransactionService : ServiceNested<CustomerBalanceTransaction>,
        INestedCreatable<CustomerBalanceTransaction, CustomerBalanceTransactionCreateOptions>,
        INestedListable<CustomerBalanceTransaction, CustomerBalanceTransactionListOptions>,
        INestedRetrievable<CustomerBalanceTransaction>
    {
        public CustomerBalanceTransactionService()
            : base(null)
        {
        }

        public CustomerBalanceTransactionService(string apiKey)
            : base(apiKey)
        {
        }

        public override string BasePath => "/v1/customers/{PARENT_ID}/customer_balance_transactions";

        public bool ExpandCustomer { get; set; }

        public virtual CustomerBalanceTransaction Create(string customerId, CustomerBalanceTransactionCreateOptions options = null, RequestOptions requestOptions = null)
        {
            return this.CreateNestedEntity(customerId, options, requestOptions);
        }

        public virtual Task<CustomerBalanceTransaction> CreateAsync(string customerId, CustomerBalanceTransactionCreateOptions options = null, RequestOptions requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.CreateNestedEntityAsync(customerId, options, requestOptions, cancellationToken);
        }

        public virtual CustomerBalanceTransaction Get(string customerId, string taxIdId, RequestOptions requestOptions = null)
        {
            return this.GetNestedEntity(customerId, taxIdId, null, requestOptions);
        }

        public virtual Task<CustomerBalanceTransaction> GetAsync(string customerId, string taxIdId, RequestOptions requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.GetNestedEntityAsync(customerId, taxIdId, null, requestOptions, cancellationToken);
        }

        public virtual StripeList<CustomerBalanceTransaction> List(string customerId, CustomerBalanceTransactionListOptions options = null, RequestOptions requestOptions = null)
        {
            return this.ListNestedEntities(customerId, options, requestOptions);
        }

        public virtual Task<StripeList<CustomerBalanceTransaction>> ListAsync(string customerId, CustomerBalanceTransactionListOptions options = null, RequestOptions requestOptions = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            return this.ListNestedEntitiesAsync(customerId, options, requestOptions, cancellationToken);
        }

        public virtual IEnumerable<CustomerBalanceTransaction> ListAutoPaging(string customerId, CustomerBalanceTransactionListOptions options = null, RequestOptions requestOptions = null)
        {
            return this.ListNestedEntitiesAutoPaging(customerId, options, requestOptions);
        }
    }
}
