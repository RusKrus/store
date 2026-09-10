using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Store.API.Contracts.Response.Cart;
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
        await context.SaveChangesAsync();

        var request = new { email = _email, password = _password };
        var loginResponse = await fixture.Client.PostAsJsonAsync("login", request);
        if (loginResponse.StatusCode != HttpStatusCode.OK) throw new UnauthorizedAccessException();

        await SeedHelpers.CreateProductAsync(
            context,
            "Test product",
            "Test description",
            10,
            10,
            _seedProductId);
    }

    [Fact]
    public async Task AddProductTest_AddsProductToCart()
    {
        var request = new { productid = _seedProductId, quantity = 1  };
        var addProductResponse = await fixture.Client.PostAsJsonAsync("api/cart/product", request);

        addProductResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedCartItemExists = await ExecuteWithContext(context =>
        {
            return Task.FromResult(context.CartItems.FirstOrDefault(ci => ci.ProductId == _seedProductId));
        });
        expectedCartItemExists.Should().NotBeNull();
    }

    [Fact]
    public async Task AddProductTest_ShowsErrorIfTooBigQuantity()
    {
        var request = new { productid = _seedProductId, quantity = 1000 };
        var response = await fixture.Client.PostAsJsonAsync("api/cart/product", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProductFromCart_RemovesProductFromCart()
    {

    }


}