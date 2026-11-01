using System.ComponentModel.DataAnnotations;

namespace Store.API.Contracts.Requests.Product;

/// <summary>
///     Description of a product to update
/// </summary>
public record UpdateProductRequest
{
    [Required]
    [Length(1, 50)]
    public string Name { get; init; }
    [Required]
    [Length(1, 200)]
    public string Description { get; init; }
    [Required]
    [Range(0.01, 999999999)]
    public decimal Price { get; init; }
    public int Quantity { get; init; } = 0;
};