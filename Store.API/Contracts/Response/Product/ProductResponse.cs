namespace Store.API.Contracts.Response;

public record ProductResponse(int Id, string Name, string Description, decimal Price, int Quantity);