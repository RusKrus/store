using Tests.Integration.Base;

namespace Tests.Integration;

[Collection("Integration-base")]
public class UnitTest1(IntegrationTestFixture fixture) : IntegrationTestBase(fixture)
{
    [Fact]
    public void Test1()
    {
    }
}