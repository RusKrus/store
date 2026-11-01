namespace Application.Common.Queries;

public record GetCartByUserQueryOptions(int UserId, bool? IncludeProductItems, bool? IncludeProducts);