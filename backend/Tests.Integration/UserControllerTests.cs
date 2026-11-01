using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Store.API.Contracts.Requests.User;
using Store.API.Contracts.Response;
using Store.API.Contracts.Response.Users;
using Store.Domain.Enums;
using Store.Infrastructure.Persistence;
using Tests.Integration.Base;
using Tests.Integration.Data;
using Tests.Integration.TestUtils;

namespace Tests.Integration;

[Collection("Integration-base")]
public sealed class UserControllerTests(IntegrationTestFixture fixture) : IntegrationTestBase(fixture)
{
    private readonly UsersData.AddUserData _firstUser = new (
        "First", 
        "Customer", 
        "firstCustomer@gmail.com", 
        "strong_password", 
        UserRole.Customer);
    
    private readonly UsersData.AddUserData _secondUser = new (
        "Second", 
        "Customer", 
        "secondCustomer@gmail.com", 
        "strong_password", 
        UserRole.Customer);
    
    private readonly UsersData.AddUserData _firstAdmin = new (
        "First", 
        "Admin", 
        "firstAdmin@gmail.com", 
        "strong_password", 
        UserRole.Admin);

    protected override async Task SeedDataAsync(StoreContext context)
    {
        await SeedHelpers.AddUserAsync(context, _firstUser);
        await SeedHelpers.AddUserAsync(context, _secondUser);
        await SeedHelpers.AddUserAsync(context, _firstAdmin);

        await context.SaveChangesAsync();
    }

    #region GetUsers tests
    [Fact]
    public async Task GetAllUsersTest_ReturnsThreeUsers()
    {
        using var client = Fixture.Factory.CreateClient();

        var response = await client.GetAsync("api/user");
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await ReadJsonAsync<PaginationResponse<UserResponse>>(response);
        result.Should().NotBeNull();
        result.Items.Count.Should().Be(3);
        result.Total.Should().Be(3);

        var firstUserValidationResult = ValidateUser(result.Items[0], _firstUser);
        var secondResultValidationResult = ValidateUser(result.Items[1], _secondUser);
        var thirdResultValidationResult = ValidateUser(result.Items[2],_firstAdmin);

        firstUserValidationResult.Should().Be(true);
        secondResultValidationResult.Should().Be(true);
        thirdResultValidationResult.Should().Be(true);
    }
    #endregion

    #region GetUserByEmailTests
    [Fact]
    public async Task GetUserByEmailTest_ReturnsRequiredUser()
    {
        using var client = Fixture.Factory.CreateClient();

        var response = await client.GetAsync("api/user/by-email?email=firstCustomer@gmail.com");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await ReadJsonAsync<UserResponse>(response);
        result.Should().NotBeNull();

        var validationResult = ValidateUser(result,_firstUser);

        validationResult.Should().Be(true);

    }

    [Fact]
    public async Task GetUserByEmailTest_ReturnsNotFoundIfNoSuchUser()
    {
        using var client = Fixture.Factory.CreateClient();

        var response = await client.GetAsync("api/user/by-email?email=notExistingCustomer@gmail.com");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    #endregion

    #region GetUserByIdTest
    [Fact]
    public async Task GetUserByIdTest_ReturnsRequiredUser()
    {
        using var client = Fixture.Factory.CreateClient();
        var requiredUser = await ExecuteWithContext(async context => await context.Users.FirstAsync(u => u.Email == _firstUser.Email));

        var response = await client.GetAsync($"api/user/{requiredUser.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var foundUser = await ReadJsonAsync<UserResponse>(response);
        foundUser.Should().NotBeNull();
        var isRequiredUser = foundUser.Email == requiredUser.Email;
        isRequiredUser.Should().BeTrue();
    }


    [Fact]
    public async Task GetUserByIdTest_ReturnsNotFoundIfNoSuchUser()
    {
        using var client = Fixture.Factory.CreateClient();

        var response = await client.GetAsync("api/user/213122312130000");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
    #endregion

    #region CreateUserTests
[Fact]
    public async Task CreateUserTest_CreatesAndReturnsNewUserId()
    {
        using var client = Fixture.Factory.CreateClient();
        await AuthAsAdmin(client);
        var request = new CreateUserRequest
        {
            FirstName = "Third",
            LastName = " User",
            Email = "thirdUserEmail@gmail.com",
            Password = "strong_password",
            Role = UserRole.Customer
        };

        var response = await client.PostAsJsonAsync("api/user", request);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var createdId = await ReadJsonAsync<int>(response);
        var createdUser = await ExecuteWithContext(async context => await context.Users.FirstOrDefaultAsync(u =>u.Id == createdId));
        createdUser.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateUserTest_UserCantCreateAnotherUser()
    {
        using var client = Fixture.Factory.CreateClient();
        await AuthAsUser(client);

        var request = new CreateUserRequest
        {
            FirstName = "Fourth",
            LastName = " User",
            Email = "fourthUserEmail@gmail.com",
            Password = "strong_password",
            Role = UserRole.Customer
        };

        var response = await client.PostAsJsonAsync("api/user", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserTest_AdminCantCreateAnotherAdmin()
    {
        using var client = Fixture.Factory.CreateClient();
        await AuthAsAdmin(client);

        var request = new CreateUserRequest
        {
            FirstName = "Second",
            LastName = " Admin",
            Email = "secondAdminEmail@gmail.com",
            Password = "strong_password",
            Role = UserRole.Admin
        };

        var response = await client.PostAsJsonAsync("api/user", request);
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUserTest_CantUserWithSameEmail()
    {
        using var client = Fixture.Factory.CreateClient();
        await AuthAsAdmin(client);

        var request = new CreateUserRequest
        {
            FirstName = _firstUser.FirstName,
            LastName = _firstUser.LastName,
            Email = _firstUser.Email,
            Password = "strong_password",
            Role = _firstUser.Role
        };

        var response = await client.PostAsJsonAsync("api/user", request);
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
    #endregion

    #region GetProfileTests
    [Fact]
    public async Task GetSelfProfileTest_ReturnsSelfData()
    {
        using var client = Fixture.Factory.CreateClient();
        await AuthAsUser(client);

        var response = await client.GetAsync("api/user/profile");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await ReadJsonAsync<UserResponse>(response);
        user.Should().NotBeNull();
        var isSameUser = user.Email == _firstUser.Email;
        isSameUser.Should().BeTrue();
    }

    [Fact]
    public async Task GetSelfProfileTest_ReturnsUnauthorizedIfSo()
    {
        using var client = Fixture.Factory.CreateClient();

        var response = await client.GetAsync("api/user/profile");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
    #endregion

    #region DeleteUserTests
    // [Fact]
    // public async Task DeleteUserTest_DeletesUser()
    // {
    //     using var client = Fixture.Factory.CreateClient();
    //     await AuthAsAdmin(client);

    //     var userToDelete = 

    //     var response = await client.DeleteAsync()
    // }
    #endregion

    #region helpers
    private bool ValidateUser(UserResponse? user, string name, string email, UserRole role)
    {
        if (user is null) return false;
        return user.FullName == name && user.Email == email && user.Role == role;
    }

    private bool ValidateUser(UserResponse? user, UsersData.AddUserData existingUser)
    {
        if (user is null) return false;
        return user.FullName == existingUser.FullName && user.Email == existingUser.Email && user.Role == existingUser.Role;
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
    #endregion
    
}