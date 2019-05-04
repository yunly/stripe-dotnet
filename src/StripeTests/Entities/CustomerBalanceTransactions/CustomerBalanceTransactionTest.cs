namespace StripeTests
{
    using Newtonsoft.Json;
    using Stripe;
    using Xunit;

    public class CustomerBalanceTransactionTest : BaseStripeTest
    {
        public CustomerBalanceTransactionTest(StripeMockFixture stripeMockFixture)
            : base(stripeMockFixture)
        {
        }

        [Fact]
        public void Deserialize()
        {
            string json = this.GetFixture("/v1/customers/cus_123/customer_balance_transactions/cbtxn_123");
            var taxId = JsonConvert.DeserializeObject<CustomerBalanceTransaction>(json);
            Assert.NotNull(taxId);
            Assert.IsType<CustomerBalanceTransaction>(taxId);
            Assert.NotNull(taxId.Id);
            Assert.Equal("customer_balance_transaction", taxId.Object);
        }

        [Fact]
        public void DeserializeWithExpansions()
        {
            string[] expansions =
            {
              "customer",
              "invoice",
            };

            string json = this.GetFixture("/v1/customers/cus_123/customer_balance_transactions/cbtxn_123", expansions);
            var taxId = JsonConvert.DeserializeObject<CustomerBalanceTransaction>(json);
            Assert.NotNull(taxId);
            Assert.IsType<CustomerBalanceTransaction>(taxId);
            Assert.NotNull(taxId.Id);
            Assert.Equal("customer_balance_transaction", taxId.Object);

            Assert.NotNull(taxId.Customer);
            Assert.Equal("customer", taxId.Customer.Object);

            Assert.NotNull(taxId.Invoice);
            Assert.Equal("invoice", taxId.Invoice.Object);
        }
    }
}
