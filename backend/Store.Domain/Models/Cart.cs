using Domain.Common;

namespace Store.Domain.Models;

public class Cart : BaseEntity
{
  private Cart() {}

  public Cart(int? userId)
  {
    UserId = userId;
  }
  public int? UserId { get; private set; }
  public Guid CartGuid { get; private set; } = Guid.NewGuid();
  public User User { get; private set; } = null!;
  public List<CartItem> CartItems { get; private set; } = [];
  public decimal TotalPrice => CartItems.Sum(ci => ci?.TotalPrice ?? 0);
  public DateTime CreatedAt { get; } = DateTime.UtcNow;
  public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

  public void AddProduct(int productId, int quantity)
  {
    var existingCartItem = CartItems.FirstOrDefault(ci => ci.ProductId == productId);
    if (existingCartItem is null)
    {
      var newCartItem = new CartItem(productId, quantity);
      CartItems.Add(newCartItem);
    }
    else
    {
      existingCartItem.UpdateQuantity(quantity);
    }
    UpdatedAt = DateTime.UtcNow;
  }

  public bool RemoveCartItem(int productId)
  {
    var desiredCartItem = CartItems.FirstOrDefault(ci => ci.ProductId == productId);
    if (desiredCartItem is null) return false;
    var result = CartItems.Remove(desiredCartItem);
    UpdatedAt = DateTime.UtcNow;
    return result;
  }

  public void MergeCarts(Cart newCart)
  {
    foreach (var newCartItem in newCart.CartItems)
    {
      var cartItem = CartItems.FirstOrDefault(ci => ci.ProductId == newCartItem.ProductId);
      if (cartItem is null)
      {
        AddProduct(newCartItem.ProductId, newCartItem.Quantity);
      }
      else
      {
        cartItem.UpdateQuantity(cartItem.Quantity + newCartItem.Quantity);
      }
    }
    UpdatedAt = DateTime.UtcNow;
  }

  public void ClearCart()
  {
    CartItems.Clear();
    UpdatedAt = DateTime.UtcNow;
  }
}
