using Application.Common.PaginationOptions;

namespace Application.Commands;

public record GetPaginatedProductsQuery(PaginationOptions options, string? searchString);