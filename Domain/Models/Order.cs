using Domain.Common;
using Store.Domain.Enums;

namespace Store.Domain.Models;

public class Order : BaseEntity
{
    private Order() {}

    public Order(
        int userId,
        decimal totalPrice,
        DateTime shippingAt,
        OrderReceiverInfo receiverInfo,
        List<OrderItem>? orderItems = null)
    {
        UserId = userId;
        if (orderItems is not null) OrderItems = orderItems ;
        TotalPrice = totalPrice;
        ShippingAt = shippingAt;
        OrderReceiverInfo = receiverInfo;

        Status = OrderStatuses.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public int UserId { get; private set; }
    public User User { get; private set; } = null!;
    public List<OrderItem> OrderItems { get; private set; } = [];
    public decimal TotalPrice { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public OrderStatuses Status { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime ShippingAt { get; private set; }
    public OrderReceiverInfo OrderReceiverInfo { get; private set; }
    public void ChangeStatus(OrderStatuses status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddOrderItem(int productId, int orderId, decimal price, int quantity)
    {
        var newOrderItem = new OrderItem(productId, orderId, price, quantity);
        OrderItems.Add(newOrderItem);
    }
}

public record OrderReceiverInfo(
    string ReceiverName,
    string ReceiverLastname,
    string ReceiverAddress,
    string ReceiverEmail,
    string ReceiverPhoneNumber,
    string City,
    string Address,
    string? OrderComment);
