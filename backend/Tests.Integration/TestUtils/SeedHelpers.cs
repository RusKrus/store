using Store.Domain.Enums;
using Store.Domain.Models;
using Store.Infrastructure.Persistence;
using Tests.Integration.Data;

namespace Tests.Integration.TestUtils;

public static class SeedHelpers
{
    public static async Task<User> AddUserAsync(StoreContext context, MockData.AddUserData data)
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

    public static async Task CreateProductAsync(StoreContext context, MockData.AddProductData productToAdd)
    {
        Product product;
        if (productToAdd.Id is not null) product = new Product(
            productToAdd.Name, 
            productToAdd.Description, 
            productToAdd.Price, 
            productToAdd.Quantity, 
            productToAdd.Category)
        {
            Id = productToAdd.Id.Value
        };
        else product = new Product(productToAdd.Name, productToAdd.Description, productToAdd.Price, productToAdd.Quantity, productToAdd.Category);
        
        await context.Products.AddAsync(product);
    }

    public static async Task CreateProductAsync(
        StoreContext context,
        string name,
        string description,
        decimal price,
        int quantity,
        ProductCategory category,
        int? id = null)
    {

        Product product;

        if (id is not null) product = new Product(name, description, price, quantity, category) { Id = id.Value };
        else product = new Product(name, description, price, quantity, category);
        
        await context.Products.AddAsync(product);
    }

    public static void CreateCartItem(StoreContext context, int userId, int productId, int quantity)
    {
        var cart = new Cart(userId);
        cart.AddProduct(productId, quantity);
        context.Carts.Add(cart);
    }
}