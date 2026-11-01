using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Store.Domain.Models;


namespace Store.Application.Interfaces;

public interface IProductRepository : IBaseRepository<Product>
{
    Task<PaginationResult<Product>> GetPaginatedListAsync(
        PaginationOptions options,
        string? searchString,
        CancellationToken cancellationToken = default
        );
}