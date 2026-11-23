using Store.Domain.Enums;
using Store.Domain.Models;

namespace Store.Application.Requests.Commands.Product;

public record UpdateProductCommand (
    int Id, 
    string Name, 
    string Description, 
    ProductSpecGroup[] Specs,
    string Details,
    decimal Price, 
    int Quantity, 
    ProductCategory Category);