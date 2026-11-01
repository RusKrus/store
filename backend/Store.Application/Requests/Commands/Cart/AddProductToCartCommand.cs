namespace Application.Commands.Cart;

public record AddProductToCartCommand(int ProductId, int Quantity);