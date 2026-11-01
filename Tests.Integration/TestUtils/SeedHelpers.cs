using System.Net.Http.Headers;
using Store.Domain.Enums;
using Store.Domain.Models;
using Store.Infrastructure.Persistence;
using Tests.Integration.Fakes;
using Tests.Integration.Data;

namespace Tests.Integration.TestUtils;

public static class SeedHelpers
{
    public static async Task<User> AddUserAsync(StoreContext context, UsersData.AddUserData data)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(data.Password);
        var newUser = new User(data.FirstName, data.LastName, data.Email, hashedPassword, data.Role);
        await context.Users.AddAsync(newUser);
        return newUser;
    }
    
    public static async Task<User> AddUserAsync(
        StoreContext context,
        string firstName,
        string lastName,
        string email,
        string password,
        UserRole role = UserRole.Admin)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var newUser = new User(firstName, lastName, email, hashedPassword, role);
        await context.Users.AddAsync(newUser);
        return newUser;
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

    public static void AuthenticateAsAsync(
        this HttpClient client,
        int userId,
        string userEmail,
        string fullName,
        UserRole role)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.SchemaName);
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserIdHeader, userId.ToString());
        client.DefaultRequestHeaders.Add(TestAuthHandler.EmailHeader, userEmail);
        client.DefaultRequestHeaders.Add(TestAuthHandler.NameHeader, fullName);
        client.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeader, role.ToString());
    }

    public static void CreateCartItem(StoreContext context, int userId, int productId, int quantity)
    {
        var cart = new Cart(userId);
        cart.AddProduct(productId, quantity);
        context.Carts.Add(cart);
    }
}