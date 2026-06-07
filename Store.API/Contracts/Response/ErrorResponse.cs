namespace Store.API.Contracts.Response;

public record ErrorResponse(string message, string? StackTrace = null);