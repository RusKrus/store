namespace Store.API.Contracts.Response.OrderItem;

public record OrderItemResponse(int Id, int ProductId, int OrderId, decimal ItemPrice, int Quantity);