using Store.Domain.Enums;

namespace Application.Commands;

public record UpdateProductCommand (int Id, string Name,  string Description, decimal Price, int Quantity, ProductCategory Category);