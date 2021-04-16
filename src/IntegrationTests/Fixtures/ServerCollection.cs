using Xunit;

namespace IntegrationTests.Fixtures
{
    [CollectionDefinition("ServerCollection")]
    public class ServerCollection : ICollectionFixture<ServerCollectionFixture>
    {
    }
}
