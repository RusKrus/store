namespace Application.Common.Queries;

public record GetCartByGuidQueryOptions(Guid CartGuid, bool? IncludeProductItems, bool? IncludeProducts);