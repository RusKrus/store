using Domain.Common;

namespace Store.Domain.Models;

public class OrderItem : BaseEntity
{
  public int ProductId { get; private set; }
  public Product Product { get; private set; } = null!;
  public int OrderId { get; private set; }
  public Order Order { get; private set; } = null!;
  public decimal Price { get; private set; }
  public int Quantity { get; private set; }
}
