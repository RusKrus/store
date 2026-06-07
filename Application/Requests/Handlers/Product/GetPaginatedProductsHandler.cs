using Application.Commands;
using Application.Common.PaginationResult;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers;

public class GetPaginatedProductsHandler(IProductRepository productRepository)
{
    public async Task<PaginationResult<Product>> Handle(GetPaginatedProductsQuery query, CancellationToken ct)
    {
        return await productRepository.GetPaginatedListAsync(query.options, query.searchString, ct);
    }
}