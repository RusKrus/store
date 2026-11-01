using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Store.Application.Interfaces;
using Store.Infrastructure.Extensions;
using Store.Domain.Models;
using Store.Infrastructure.Persistence;


namespace Store.Infrastructure.Repositories;

public class ProductRepository(StoreContext context) : BaseRepository<Product>(context), IProductRepository
{
    public async Task<PaginationResult<Product>> GetPaginatedListAsync(
        PaginationOptions options,
        string? searchString,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = DbSet;
        if (searchString != null)
        {
            query = query.Where(p => p.Name.ToLower().Contains(searchString.ToLower()));
        }
        return await query.PaginateAsync(options, cancellationToken);
    }
}