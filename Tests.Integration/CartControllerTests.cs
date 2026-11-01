using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Store.Domain.Enums;
using Store.Infrastructure.Persistence;
using Tests.Integration.Base;
using Tests.Integration.TestUtils;

namespace Tests.Integration;

[Collection("Integration-base")]
public sealed class CartControllerTests(IntegrationTestFixture fixture) : IntegrationTestBase(fixture)
{
    private readonly string _firstName = "First";
    private readonly string _lastName = "Last";
    private readonly string _email = "test@gmail.com";
    private readonly string _password = "strong_password";
    private int? _userId;
    
    private readonly int _seedProductId = 1;

    private string FullName => $"{_firstName} {_lastName}";

    protected override async Task SeedDataAsync(StoreContext context)
    {
        var user = await SeedHelpers.AddUserAsync(context, _firstName, _lastName, _email, _password);
        await context.SaveChangesAsync();
        _userId = user.Id;

        await SeedHelpers.CreateProductAsync(
            context,
            "Test product",
            "Test description",
            10,
            10,
            _seedProductId);
    }

    [Fact]
    public async Task AddProductTest_AddsProductToAuthorizedUserCart()
    {
        using var client = Fixture.Factory.CreateClient();
        Authorize(client);
        var request = new { productId = _seedProductId, quantity = 1  };
        var addProductResponse = await client.PostAsJsonAsync("api/cart/product", request);

        addProductResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedCartItemExists = await ExecuteWithContext(context =>
        {
            return Task.FromResult(context.CartItems.FirstOrDefault(ci => ci.ProductId == _seedProductId));
        });
        expectedCartItemExists.Should().NotBeNull();
    }

    [Fact]
    public async Task AddProductTest_SavesCartIdInCookieForUnauthorizedUser()
    {
        using var client = Fixture.Factory.CreateClient();
        var request = new { productId = _seedProductId, quantity = 1  };
        var addProductResponse = await client.PostAsJsonAsync("api/cart/product", request);

        addProductResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var expectedCartItemExists = await ExecuteWithContext(context =>
        {
            return Task.FromResult(context.CartItems.FirstOrDefault(ci => ci.ProductId == _seedProductId));
        });
        expectedCartItemExists.Should().NotBeNull();

        var cartCookie = addProductResponse.Headers
            .GetValues("Set-Cookie")
            .FirstOrDefault(c => c.Contains("cartId"));

        var cartId = cartCookie?.Split(';')[0].Split('=').Last();

        var isCartIdInCookies = await ExecuteWithContext(context =>
        {
            var cart = context.Carts.FirstOrDefault(c => c.CartGuid.ToString() == cartId);
            return Task.FromResult(cart is not null);
        });

        isCartIdInCookies.Should().BeTrue();
    }

    [Fact]
    public async Task AddProductTest_ShowsErrorIfTooBigQuantity()
    {
        using var client = Fixture.Factory.CreateClient();
        Authorize(client);
        var request = new { productId = _seedProductId, quantity = 1000 };
        var response = await client.PostAsJsonAsync("api/cart/product", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteProductFromCart_RemovesProductFromAuthorizedUserCart()
    {
        using var client = Fixture.Factory.CreateClient();
        var userId = Authorize(client);
        await ExecuteWithContext(async context =>
        {
            SeedHelpers.CreateCartItem(context, userId, _seedProductId, 1);
            await context.SaveChangesAsync();
        });

        var response = await client.DeleteAsync($"api/cart/product/{_seedProductId}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private int Authorize(HttpClient client)
    {
        var userId = _userId ?? throw new ArgumentNullException(nameof(_userId));
        client.AuthenticateAsAsync(userId, _email, FullName, UserRole.Admin);
        return userId;
    }
}