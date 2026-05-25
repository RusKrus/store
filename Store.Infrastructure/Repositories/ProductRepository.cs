using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Infrastructure.Extensions;
using Store.Domain.Models;
using Store.Infrastructure.Persistence;


namespace Store.Infrastructure.Repositories;

public class ProductRepository(StoreContext context) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        return await context.Products.FirstOrDefaultAsync(p => p.Id == productId, cancellationToken);
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await context.Products.AddAsync(product, cancellationToken);
    }

    public async Task DeleteByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        await context.Products.Where(p => p.Id == productId).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<PaginationResult<Product>> GetPaginatedListAsync(PaginationOptions options, string? searchString, CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = context.Products;
        if (searchString != null)
        {
            query = query.Where(p => p.Name.ToLower().Contains(searchString.ToLower()));
        }
        return await query.PaginateAsync(options, cancellationToken);
    }
}