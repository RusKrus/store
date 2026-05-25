namespace Application.Commands;

public record UpdateProductCommand (int Id, string Name,  string Description, decimal Price, int Quantity);