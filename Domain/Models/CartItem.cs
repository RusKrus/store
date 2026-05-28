using Domain.Common;

namespace Store.Domain.Models;

public class CartItem : BaseEntity
{
  public int ProductId { get; private set; }
  public Product Product { get; private set; } = null!;
  public int CartId { get; private set; }
  public Cart Cart { get; private set; } = null!;
  public int Quantity { get; private set; }
}
