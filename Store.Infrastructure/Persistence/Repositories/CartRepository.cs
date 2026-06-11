using Application.Common.Queries;
using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Domain.Models;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public class CartRepository(StoreContext context) : BaseRepository<Cart>(context), ICartRepository
{
    public async Task<Cart?> GetByUserIdAsync(
        GetCartByUserQueryOptions options,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Cart> query = DbSet;

        if (options.IncludeProducts == true)
        {
            query = query.Include(c => c.CartItems).ThenInclude(ci => ci.Product);
        }
        else if (options.IncludeProductItems == true)
        {
            query = query.Include(c => c.CartItems);
        }

        var cart = await query.FirstOrDefaultAsync(c => c.UserId == options.UserId, cancellationToken);
        return cart;
    }

    public async Task<Cart?> GetByGuidAsync(GetCartByGuidQueryOptions options, CancellationToken cancellationToken = default)
    {
        IQueryable<Cart> query = DbSet;

        if (options.IncludeProducts == true)
        {
            query = query.Include(c => c.CartItems).ThenInclude(ci => ci.Product);
        }
        else if (options.IncludeProductItems == true) query = query.Include(c => c.CartItems);

        var cart = await query.FirstOrDefaultAsync(c => c.CartGuid == options.CartGuid, cancellationToken);
        return cart;
    }
}