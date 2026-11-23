using System.Text.Json;
using Domain.Common;
using Store.Domain.Enums;

namespace Store.Domain.Models;

public class Product : BaseEntity
{
  private Product() {}

  public Product(
    string name,
    string description,
    string details,
    ProductSpecGroup[] specs,
    decimal price,
    int quantity,
    ProductCategory category)
  {
    SetValues(name, description, details, specs, price, quantity, category);
  }

  public string Name { get; private set; } = "";
  public string Description { get; private set; } = "";
  public string Details { get; private set; } = "";
  public decimal Price { get; private set; }
  public ProductCategory Category { get; private set; }
  public int Quantity { get; private set; }

  public string Specs { get; private set; } = "[]";

  public List<ImageDescription> Images { get; private set; } = new();

  public void Update(
    string name,
    string description,
    string details,
    ProductSpecGroup[] specs,
    decimal price,
    int quantity,
    ProductCategory category)
  {
    SetValues(name, description, details, specs, price, quantity, category);
  }

  private void SetValues(
    string name, 
    string description, 
    string details,
    ProductSpecGroup[] specs,
    decimal price,
    int quantity, 
    ProductCategory category)
  {
    if (name.Length < 1)
    {
      throw new ArgumentException("Product name cannot be empty");
    }

    if (description.Length < 1)
    {
      throw new ArgumentException("Product description cannot be empty");
    }

    if (details.Length < 200)
    {
      throw new ArgumentException("Product details should be no less then 200 ch");
    }

    if (specs is null)
    {
      throw new ArgumentException("Specs are not specified");
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
    Details = details;
    Specs = JsonSerializer.Serialize(specs);
    Price = price;
    Quantity = quantity;
    Category = category;
    
  }

  private void SetImageKey(List<ImageDescription> images)
  {
    Images = images;
  }
}
