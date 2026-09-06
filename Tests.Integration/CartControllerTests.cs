using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Store.Infrastructure.Persistence;
using Tests.Integration.Base;
using Tests.Integration.TestUtils;

namespace Tests.Integration;

[Collection("Integration-base")]
public class CartControllerTests(IntegrationTestFixture fixture) : IntegrationTestBase(fixture)
{
    private readonly string _firstName = "First";
    private readonly string _lastName = "Last";
    private readonly string _email = "test@gmail.com";
    private readonly string _password = "strong_password";

    private readonly int _seedProductId = 1;
    protected override async Task SeedDataAsync(StoreContext context)
    {
        await SeedHelpers.AddUserAsync(context, _firstName, _lastName, _email, _password);
        var request = new { email = _email, password = _password };
        await fixture.Client.PostAsJsonAsync("login", request);
        await SeedHelpers.CreateProductAsync(
            context,
            "Test product",
            "Test description",
            10,
            10,
            _seedProductId);
    }

    [Fact]
    public async Task AddProductTest_ShowsNewProductInTheCart()
    {
        var request = new { productid = _seedProductId, quantity = 1  };
        var response = await fixture.Client.PostAsJsonAsync("api/cart/product", request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddProductTest_ShowsErrorIfTooBigQuantity()
    {
        var request = new { productid = _seedProductId, quantity = 1000 };
        var response = await fixture.Client.PostAsJsonAsync("api/cart/product", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}