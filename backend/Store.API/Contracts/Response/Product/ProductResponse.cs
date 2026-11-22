using Store.API.Contracts.Response.Common;
using Store.Domain.Enums;
using Store.Domain.Models;

namespace Store.API.Contracts.Response.Product;

public record ProductResponse(
    int Id,
    string Name,
    string Description,
    string Details,
    ProductSpecGroup[] Specs,
    decimal Price,
    int Quantity,
    List<ImageResponse> Images,
    ProductCategory Category);