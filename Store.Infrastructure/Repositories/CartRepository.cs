using Application.Common.Queries;
using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Domain.Models;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public class CartRepository(StoreContext context) : BaseRepository<Cart>(context), ICartRepository
{
    public async Task<Cart?> GetByUserIdAsync(
        GetCartQueryOptions options,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Cart> query = DbSet;

        if (options.IncludeProducts == true) query = query.Include(c => c.CartItems).ThenInclude(ci => ci.Product);
        else if (options.IncludeProductItems == true) query = query.Include(c => c.CartItems);

        var cart = await query.FirstOrDefaultAsync(c => c.UserId == options.UserId, cancellationToken);
        return cart;
    }
}