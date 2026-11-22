using Store.Domain.Enums;
using Store.Domain.Models;

namespace Store.Application.Requests.Commands.Product;

public record CreateProductCommand(
    string Name,  
    string Description,
    string Details,
    ProductSpecGroup[] Specs,
    decimal Price,
    int Quantity,
    ProductCategory Category);