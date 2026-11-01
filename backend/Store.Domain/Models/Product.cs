using Domain.Common;

namespace Store.Domain.Models;

public class Product : BaseEntity
{
  private Product() {}
  public Product(string name, string description, decimal price, int quantity)
  {
    SetValues(name, description, price, quantity);
  }

  public void Update(string name, string description, decimal price, int quantity)
  {
    SetValues(name, description, price, quantity);
  }
  public string Name { get; private set; } = null!;
  public string Description { get; private set; } = null!;
  public decimal Price { get; private set; }
  public int Quantity { get; private set; }

  private void SetValues(string name, string description, decimal price, int quantity)
  {
    if (name.Length < 1)
    {
      throw new ArgumentException("Product name cannot be empty");
    }

    if (description.Length < 1)
    {
      throw new ArgumentException("Product description cannot be empty");
    }

    if (price < 0)
    {
      throw new ArgumentException("Product price cannot be negative");
    }
    if (quantity < 0)
    {
      throw new ArgumentException("Product quantity cannot be negative");
    }
    Name = name;
    Description = description;
    Price = price;
    Quantity = quantity;
  }
}
