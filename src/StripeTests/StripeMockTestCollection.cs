namespace StripeTests
{
    using Xunit;

    [CollectionDefinition("stripe-mock tests")]
    public class StripeMockTestCollection :

        // The order here is important, StripeMockFixture creates a custom StripeClient instance,
        // and MockHttpClientFixture modifies the HttpClient within the StripeClient.
        ICollectionFixture<StripeMockFixture>,
        ICollectionFixture<MockHttpClientFixture>
    {
        // This class has no code, and is never created. Its purpose is simply to be the place to
        // apply [CollectionDefinition] and all the ICollectionFixture<> interfaces.
    }
}
