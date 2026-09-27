using System.Net;
using System.Net.Http.Json;
using Store.Infrastructure.Persistence;
using Tests.Integration.Base;
using FluentAssertions;
using Tests.Integration.TestUtils;

namespace Tests.Integration;

[Collection("Integration-base")]
public class AuthorizationControllerTests(IntegrationTestFixture fixture) : IntegrationTestBase(fixture)
{
    private readonly string _firstName = "First";
    private readonly string _lastName = "Last";
    private readonly string _email = "test@gmail.com";
    private readonly string _password = "strong_password";
    protected override async Task SeedDataAsync(StoreContext context)
    {
        await SeedHelpers.AddUserAsync(context, _firstName, _lastName, _email, _password);
    }

    [Fact]
    public async Task LoginTest_ReturnsOk()
    {
        var client = fixture.Factory.CreateClient();
        var request = new { email = "test@gmail.com", password = "strong_password" };
        var response = await client.PostAsJsonAsync("login", request);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadAsStringAsync();
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginTest_ReturnsErrorForWrongCredentials()
    {
        var client = fixture.Factory.CreateClient();
        var request = new { email = "doesNotExists@gmail.com", password = "notExistingPassword" };
        var response = await client.PostAsJsonAsync("login", request);
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Register_ReturnsOk()
    {
        var client = fixture.Factory.CreateClient();
        var request = new
        {
            firstName = "testName",
            lastName = "testLatName",
            email = "testUser@example.com",
            password = "string"
        };
        var response = await client.PostAsJsonAsync("register", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadAsStringAsync();
        result.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Register_ReturnsErrorForExistingEmail()
    {
        var client = fixture.Factory.CreateClient();
        var request = new
        {
            firstName = _firstName,
            lastName = _lastName,
            email = _email,
            password = _password
        };
        var response = await client.PostAsJsonAsync("register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}