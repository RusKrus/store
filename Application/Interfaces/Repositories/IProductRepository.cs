using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Store.Domain.Models;


namespace Store.Application.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(int productId, CancellationToken cancellationToken = default);
    Task<PaginationResult<Product>> GetPaginatedListAsync(PaginationOptions options, string? searchString, CancellationToken cancellationToken = default);
}