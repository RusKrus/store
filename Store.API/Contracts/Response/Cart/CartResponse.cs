namespace Store.API.Contracts.Response.Cart;

public record CartResponse(int Id, int UserId, List<CartItemInCartResponse> CartItems, decimal TotalPrice);