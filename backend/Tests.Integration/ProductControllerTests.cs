using System.Net;
using System.Net.Http.Json;
using Store.Domain.Enums;
using Store.Infrastructure.Persistence;
using Tests.Integration.Base;
using Tests.Integration.Data;
using Tests.Integration.TestUtils;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Store.API.Contracts.Requests.Product;
using Store.API.Contracts.Response;
using Xunit.Sdk;

namespace Tests.Integration;

[Collection("Integration-base")]
public sealed class ProductControllerTests(IntegrationTestFixture fixture) : IntegrationTestBase(fixture)
{
    private readonly MockData.AddProductData _gamingMouse = new(
        name: "Gaming Mouse",
        description: "Ergonomic wireless gaming mouse with adjustable DPI.",
        price: 59.99m,
        quantity: 25,
        category: ProductCategory.Peripherals,
        id: 1
    );

    private readonly MockData.AddProductData _mechanicalKeyboard = new(
        name: "Mechanical Keyboard",
        description: "Compact mechanical keyboard with RGB backlight.",
        price: 89.50m,
        quantity: 18,
        category: ProductCategory.Peripherals,
        id: 2
    );

    private readonly MockData.AddProductData _monitor27Inch = new(
        name: "27 Inch Monitor",
        description: "27-inch IPS monitor with 1440p resolution.",
        price: 249.00m,
        quantity: 12,
        category: ProductCategory.Monitors,
        id: 3
    );

    private readonly MockData.AddProductData _usbCHub = new(
        name: "USB C Hub",
        description: "Seven-port USB-C hub with HDMI and power delivery.",
        price: 44.99m,
        quantity: 40,
        category: ProductCategory.ComputerComponents,
        id: 4
    );

    private readonly MockData.AddUserData _firstUser = new (
        "First", 
        "Customer", 
        "firstCustomer@gmail.com", 
        "strong_password", 
        UserRole.Customer);
    
    
    private readonly MockData.AddUserData _firstAdmin = new (
        "First", 
        "Admin", 
        "firstAdmin@gmail.com", 
        "strong_password", 
        UserRole.Admin);

    protected override async Task SeedDataAsync(StoreContext context)
    {
        await SeedHelpers.CreateProductAsync(context, _gamingMouse);
        await SeedHelpers.CreateProductAsync(context, _mechanicalKeyboard);
        await SeedHelpers.CreateProductAsync(context, _monitor27Inch);

        await SeedHelpers.AddUserAsync(context, _firstUser);
        await SeedHelpers.AddUserAsync(context, _firstAdmin);
        
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetProductsTest_ReturnsSeededProducts()
    {
        var client = Fixture.Factory.CreateClient();

        var response = await client.GetAsync("api/Product");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await ReadJsonAsync<PaginationResponse<ProductResponse>>(response);
        if (content is null) throw new ArgumentNullException(nameof(content));
        content.Total.Should().Be(3);

        var items = content.Items;
        var firstItemCompareResult = ValidateProductResponse(items[0], _gamingMouse);
        var secondItemCompareResult = ValidateProductResponse(items[1], _mechanicalKeyboard);
        var thirdItemCompareResult = ValidateProductResponse(items[2], _monitor27Inch);

        firstItemCompareResult.Should().BeTrue();
        secondItemCompareResult.Should().BeTrue();
        thirdItemCompareResult.Should().BeTrue();
    }

    [Fact]
    public async Task GetProductsTest_ReturnsSearchedProducts()
    {
        var client = Fixture.Factory.CreateClient();

        var pathWithQuery = $"api/Product?searchString={Uri.EscapeDataString("27 Inch Monitor")}";
        var response = await client.GetAsync(pathWithQuery);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await ReadJsonAsync<PaginationResponse<ProductResponse>>(response);
        if (content is null) throw new ArgumentNullException(nameof(content));
        content.Total.Should().Be(1);

        var items = content.Items;
        var firstItemCompareResult = ValidateProductResponse(items[0], _monitor27Inch);

        firstItemCompareResult.Should().BeTrue();
    }

    [Fact]
    public async Task GetProductByIdTest_ReturnsSearchedProduct()
    {
        var client = Fixture.Factory.CreateClient();

        var response = await client.GetAsync($"api/Product/{_gamingMouse.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await ReadJsonAsync<ProductResponse>(response);
        if (content is null) throw new ArgumentNullException(nameof(content));

        var itemCompareResult = ValidateProductResponse(content, _gamingMouse);

        itemCompareResult.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteProductById_UserCantDeleteProduct()
    {
        var client = Fixture.Factory.CreateClient();
        await AuthAsUser(client);

        var response = await client.DeleteAsync($"api/Product/{_gamingMouse.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        await ExecuteWithContext(async context => await context.Products.FirstAsync(p => p.Id == _gamingMouse.Id));
    }

    [Fact]
    public async Task DeleteProductById_AdminDeletesProduct()
    {
        var client = Fixture.Factory.CreateClient();
        await AuthAsAdmin(client);

        var response = await client.DeleteAsync($"api/Product/{_gamingMouse.Id}");
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var deletedProduct = await ExecuteWithContext(async context => await context.Products.FirstOrDefaultAsync(p => p.Id == _gamingMouse.Id));
        deletedProduct.Should().BeNull();
    }

    [Fact]
    public async Task CreateProductAsync_UserCantCreateProduct()
    {
        var client = Fixture.Factory.CreateClient();
        await AuthAsUser(client);

        var body = new CreateProductRequest
        {
            Name = _usbCHub.Name,
            Description = _usbCHub.Description,
            Price = _usbCHub.Price,
            Category = _usbCHub.Category,
            Quantity = _usbCHub.Quantity
        };

        var response = await client.PostAsJsonAsync("api/Product", body);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateProductAsync_AdminCreatesProduct()
    {
        var client = Fixture.Factory.CreateClient();
        await AuthAsAdmin(client);

        var body = new CreateProductRequest
        {
            Name = _usbCHub.Name,
            Description = _usbCHub.Description,
            Price = _usbCHub.Price,
            Category = _usbCHub.Category,
            Quantity = _usbCHub.Quantity
        };

        var response = await client.PostAsJsonAsync("api/Product", body);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        // Assume name and description combination is unique
        await ExecuteWithContext(async context => await context.Products.FirstAsync(p => p.Name == _usbCHub.Name && p.Description == _usbCHub.Description));
    }

    [Fact]
    public async Task UpdateProduct_AdminUpdatesProduct()
    {
        var client = Fixture.Factory.CreateClient();
        await AuthAsAdmin(client);

        var body = new UpdateProductRequest
        {
            Name = _usbCHub.Name,
            Description = _usbCHub.Description,
            Price = _usbCHub.Price,
            Category = _usbCHub.Category,
            Quantity = _usbCHub.Quantity
        };

        var response = await client.PutAsJsonAsync($"api/Product/{_gamingMouse.Id}", body);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await ExecuteWithContext(async context => await context.Products.FirstAsync(p => p.Name == _usbCHub.Name && p.Description == _usbCHub.Description));
    }

    [Fact]
    public async Task UpdateProduct_UserCantUpdateProduct()
    {
        var client = Fixture.Factory.CreateClient();
        await AuthAsUser(client);

        var body = new UpdateProductRequest
        {
            Name = _usbCHub.Name,
            Description = _usbCHub.Description,
            Price = _usbCHub.Price,
            Category = _usbCHub.Category,
            Quantity = _usbCHub.Quantity
        };

        var response = await client.PutAsJsonAsync($"api/Product/{_gamingMouse.Id}", body);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);

        var updatedProduct =
            await ExecuteWithContext(async context => await context.Products.FirstOrDefaultAsync(p => p.Name == _usbCHub.Name && p.Description == _usbCHub.Description));

        updatedProduct.Should().BeNull();
    }

    [Fact]
    public async Task GetProductCategories_ReturnsFullList()
    {
        var client = Fixture.Factory.CreateClient();
        var response = await client.GetAsync("api/Product/categories?showAll=true");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await ReadJsonAsync<List<string>>(response);
        if (content is null) throw new Exception("content is null, but expected List");

        var allCategories = Enum.GetNames<ProductCategory>().ToList();

        content.Count.Should().Be(allCategories.Count);
    }

    [Fact]
    public async Task GetProductCategories_ReturnsOnlyUsedCategories()
    {
        var client = Fixture.Factory.CreateClient();
        var response = await client.GetAsync("api/Product/categories");
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await ReadJsonAsync<List<string>>(response);
        if (content is null) throw new Exception("content is null, but expected List");

        var existingCategories = await ExecuteWithContext(async context =>
            await context.Products.Select(p => p.Category).Distinct().ToListAsync());

        content.Count.Should().Be(existingCategories.Count);
    }

    private bool ValidateProductResponse(ProductResponse gainedProduct, MockData.AddProductData initialItem)
    {
        return gainedProduct.Category == initialItem.Category && 
               gainedProduct.Description == initialItem.Description &&
               gainedProduct.Name == initialItem.Name && 
               gainedProduct.Id == initialItem.Id;
    }

    private async Task AuthAsAdmin(HttpClient client)
    {
        var requiredAdmin = await ExecuteWithContext(async context => await context.Users.FirstAsync(u => u.Email == _firstAdmin.Email)); 
        client.AuthenticateAsAsync(requiredAdmin.Id, requiredAdmin.Email, requiredAdmin.FullName, requiredAdmin.Role);
    }

    private async Task AuthAsUser(HttpClient client)
    {
        var requiredUser = await ExecuteWithContext(async context => await context.Users.FirstAsync(u => u.Email == _firstUser.Email)); 
        client.AuthenticateAsAsync(requiredUser.Id, requiredUser.Email, requiredUser.FullName, requiredUser.Role);
    }
}