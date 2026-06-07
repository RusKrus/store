using Domain.Common;

namespace Store.Domain.Models;

public class OrderItem : BaseEntity
{
  private OrderItem() {}

  public OrderItem(int productId, int orderId, decimal itemPrice, int quantity)
  {
    ProductId = productId;
    OrderId = orderId;
    ItemPrice = itemPrice;
    Quantity = quantity;
  }

  public int ProductId { get; private set; }
  public Product Product { get; private set; } = null!;
  public int OrderId { get; private set; }
  public Order Order { get; private set; } = null!;
  public decimal ItemPrice { get; private set; }
  public decimal TotalPrice => ItemPrice * Quantity;
  public int Quantity { get; private set; }
}
