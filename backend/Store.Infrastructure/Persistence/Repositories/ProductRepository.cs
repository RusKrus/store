using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Domain.Enums;
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

    public async Task<List<ProductCategory>> GetAllProductCategoryNames(bool showAll,
        CancellationToken cancellationToken = default)
    {
        if (showAll)
        {
            return Enum.GetValues<ProductCategory>().Select(c => c).ToList();
        }
        else
        {
            return await DbSet.Select(p => p.Category).Distinct().ToListAsync(cancellationToken);
        }
    }
}