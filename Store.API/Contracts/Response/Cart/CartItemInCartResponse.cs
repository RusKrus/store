namespace Store.API.Contracts.Response.Cart;

public record CartItemInCartResponse(int Id, int ProductId, string ProductName, int Quantity, decimal TotalPrice);