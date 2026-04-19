using System;

namespace Store.Domain.Models;

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public List<OrderItem>? OrderItems { get; set; } = null;
}
