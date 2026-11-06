using Store.Domain.Enums;

namespace Tests.Integration.Data;

public sealed class MockData
{
    public sealed class AddUserData(string firstName, string lastName, string email, string password, UserRole role)
    {
        public string FirstName { get; set; } = firstName;
        public string LastName { get; set; } = lastName;
        public string Email { get; set; } = email;
        public string Password { get; set; } = password;
        public UserRole Role { get; set; } = role;

    
        public string FullName => $"{FirstName} {LastName}";
    }

    public sealed class AddProductData(string name, string description, decimal price, int quantity, ProductCategory category, int? id)
    {
        public int? Id { get; set; } = id;
        public string Name { get; set; } = name;
        public string Description { get; set; } = description;
        public decimal Price { get; set; } = price;
        public int Quantity { get; set; } = quantity;
        public ProductCategory Category { get; set; } = category;
    }
}