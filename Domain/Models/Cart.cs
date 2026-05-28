using System;
using Domain.Common;

namespace Store.Domain.Models;

public class Cart : BaseEntity
{
  public int UserId { get; private set; }
  public User User { get; private set; } = null!;
  public List<CartItem>? CartItems { get; private set; } = null;
}
