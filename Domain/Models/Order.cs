using System;
using Domain.Common;

namespace Store.Domain.Models;

public class Order : BaseEntity
{
    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public List<OrderItem>? OrderItems { get; private set; } = null;
}
