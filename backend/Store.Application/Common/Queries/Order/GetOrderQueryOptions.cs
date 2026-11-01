namespace Application.Common.Queries;

public record GetOrderQueryOptions(int Id, bool? IncludeOrderItems, bool? IncludeProducts);