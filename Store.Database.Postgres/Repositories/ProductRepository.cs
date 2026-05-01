using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Domain.Models;
using Store.Database.Postgres.Persistence;


namespace Store.Database.Postgres.Repositories;

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

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        context.Products.Update(product);
        await context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task DeleteByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        await context.Products.Where(p => p.Id == productId).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<List<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Products.ToListAsync(cancellationToken);
    }
}