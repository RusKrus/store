using System.ComponentModel.DataAnnotations;

namespace Store.API.Contracts.Requests.Cart;

public record AddProductToCartRequest
{
    public int ProductId { get; init; }
    [Range(1, int.MaxValue)]
    public int Quantity { get; init; }
}