using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Store.API.Contracts.Response;
using Store.API.Contracts.Response.Users;
using Store.Domain.Enums;
using Store.Infrastructure.Persistence;
using Tests.Integration.Base;
using Tests.Integration.TestUtils;

namespace Tests.Integration;

[Collection("Integration-base")]
public sealed class UserControllerTests(IntegrationTestFixture fixture) : IntegrationTestBase(fixture)
{
    protected override async Task SeedDataAsync(StoreContext context)
    {
        await SeedHelpers.AddUserAsync(
            context, 
            "First", 
            "Customer", 
            "firstCustomer@gmail.com", 
            "strong_password", 
            UserRole.Customer);
        
        await SeedHelpers.AddUserAsync(
            context, 
            "Second", 
            "Customer", 
            "secondCustomer@gmail.com", 
            "strong_password", 
            UserRole.Customer);
        
        await SeedHelpers.AddUserAsync(
            context, 
            "First", 
            "Admin", 
            "firstAdmin@gmail.com", 
            "strong_password");

        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllUsersTest_ReturnsThreeUsers()
    {
        using var client = Fixture.Factory.CreateClient();

        var response = await client.GetAsync("api/user");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PaginationResponse<UserResponse>>();
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(3);
        result.Total.Should().Be(3);

        result.Items[0].FullName.Should().Be("First Customer");
        result.Items[0].Email.Should().Be("firstCustomer@gmail.com");
        result.Items[0].Role.Should().Be(UserRole.Customer);
        
        result.Items[1].FullName.Should().Be("Second Customer");
        result.Items[1].Email.Should().Be("secondCustomer@gmail.com");
        result.Items[1].Role.Should().Be(UserRole.Customer);
        
        result.Items[2].FullName.Should().Be("First Admin");
        result.Items[2].Email.Should().Be("firstAdmin@gmail.com");
        result.Items[2].Role.Should().Be(UserRole.Admin);
    }
}