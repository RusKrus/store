namespace Application.Common.Queries;

public record GetCartQueryOptions(int UserId, bool? IncludeProductItems, bool? IncludeProducts);