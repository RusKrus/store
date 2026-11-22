namespace Store.API.Contracts.Response.Common;

public record ImageResponse(int Id, string ImageUrl, string? Description = "");
