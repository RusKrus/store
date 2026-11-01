using Store.API.Contracts.Response.OrderItem;
using Store.Domain.Enums;

namespace Store.API.Contracts.Response.Orders;

public record OrderResponse(
    int Id,
    int UserId,
    List<OrderItemResponse> OrderItems,
    decimal TotalPrice,
    DateTime CreatedAt,
    OrderStatuses Status,
    DateTime UpdatedAt,
    DateTime ShippingAt,
    ReceiverInfoResponse OrderReceiverInfo
    );