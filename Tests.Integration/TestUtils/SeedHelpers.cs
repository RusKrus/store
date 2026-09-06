using Store.Domain.Enums;
using Store.Domain.Models;
using Store.Infrastructure.Persistence;

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
}