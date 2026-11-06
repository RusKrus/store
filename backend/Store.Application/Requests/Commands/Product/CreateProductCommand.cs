using Store.Domain.Enums;

namespace Application.Commands;

public record CreateProductCommand(string Name,  string Description, decimal Price, int Quantity, ProductCategory Category);