using System.Net.Http.Headers;
using Store.Domain.Enums;
using Store.Domain.Models;
using Store.Infrastructure.Persistence;
using Tests.Integration.Fakes;

namespace Tests.Integration.TestUtils;

public static class SeedHelpers
{
    public static async Task AddUserAsync(StoreContext context, string firstName, string lastName, string email, string password)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var newUser = new User(firstName, lastName, email, hashedPassword, UserRole.Admin);
        await context.Users.AddAsync(newUser);
    }

    public static async Task CreateProductAsync(
        StoreContext context,
        string name,
        string description,
        decimal price,
        int quantity,
        int id = 1)
    {
        var product = new Product(name, description, price, quantity) { Id = id };
        await context.Products.AddAsync(product);
    }

    public static async Task AuthenticateAsAsync(
        this HttpClient client,
        int userId,
        string userEmail,
        string fullName,
        UserRole role)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemaName);
        client.DefaultRequestHeaders.Add("UserId", userId.ToString());
        client.DefaultRequestHeaders.Add("UserEmail", userEmail);
        client.DefaultRequestHeaders.Add("Name", fullName);
        client.DefaultRequestHeaders.Add("Role", role.ToString());
    }

    public static async Task CreateCartItemAsync(StoreContext context, int productId, int quantity)
    {

    }
}