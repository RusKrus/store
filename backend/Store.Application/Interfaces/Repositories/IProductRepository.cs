using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Store.Domain.Enums;
using Store.Domain.Models;


namespace Store.Application.Interfaces;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<PaginationResult<Product>> GetPaginatedListAsync(
        PaginationOptions options,
        string? searchString,
        CancellationToken cancellationToken = default);

    Task<List<ProductCategory>> GetAllProductCategoryNames(bool showAll, CancellationToken cancellationToken = default);
}