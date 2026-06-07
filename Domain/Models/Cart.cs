using Domain.Common;

namespace Store.Domain.Models;

public class Cart : BaseEntity
{
  private Cart() {}

  public Cart(int userId)
  {
    UserId = userId;
  }
  public int UserId { get; private set; }
  public User User { get; private set; } = null!;
  public List<CartItem> CartItems { get; private set; } = [];
  public decimal TotalPrice => CartItems.Sum(ci => ci?.TotalPrice ?? 0);

  public void AddProduct(int productId, int quantity)
  {
    var newCartItem = new CartItem(productId, quantity);
    CartItems.Add(newCartItem);
  }

  public bool RemoveCartItem(int productId)
  {
    var desiredCartItem = CartItems.FirstOrDefault(ci => ci.ProductId == productId);
    if (desiredCartItem is null) return false;
    var result = CartItems.Remove(desiredCartItem);
    return result;
  }

  public void ClearCart()
  {
    CartItems.Clear();
  }
}
