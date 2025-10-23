namespace Dynamiq.API.Tests.Collections
{
    [CollectionDefinition("CartTests", DisableParallelization = true)]
    public class CartTestsCollection : ICollectionFixture<CustomWebApplicationFactory<Program>>
    {
    }
}
