using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces.Repositories;
using Store.Domain.Enums;
using Store.Domain.Models;
using Store.Infrastructure.Extensions;

namespace Store.Infrastructure.Persistence.Repositories;

public class ProductRepository(StoreContext context) : BaseRepository<Product>(context), IProductRepository
{
    public override async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(p => p.Images)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<PaginationResult<Product>> GetPaginatedListAsync(
        PaginationOptions options,
        string? searchString,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = DbSet;
        if (searchString != null)
        {
            query = query
                .Include(p => p.Images)
                .Where(p => p.Name.ToLower().Contains(searchString.ToLower()));
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