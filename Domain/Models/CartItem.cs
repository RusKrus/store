using System.ComponentModel.DataAnnotations;
using Domain.Common;

namespace Store.Domain.Models;

public class CartItem : BaseEntity
{
  private CartItem() {}

  public CartItem(int productId, int quantity)
  {
    ProductId = productId;
    Quantity = quantity;
  }

  public int ProductId { get; private set; }
  public Product Product { get; private set; } = null!;
  public int CartId { get; private set; }
  public Cart Cart { get; private set; } = null!;
  public int Quantity { get; private set; }
  public decimal TotalPrice => Quantity * Product.Price;

  public void UpdateQuantity(int quantity)
  {
    if (quantity < 1)
    {
      throw new ValidationException("Quantity should be more than 0");
    }
    Quantity = quantity;
  }
}
