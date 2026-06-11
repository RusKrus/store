using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Application.Common.Queries;
using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Domain.Enums;
using Store.Domain.Models;
using Store.Infrastructure.Extensions;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public class OrderRepository(StoreContext context) : BaseRepository<Order>(context), IOrderRepository
{
    public async Task<PaginationResult<Order>> GetPaginatedListAsync(
        PaginationOptions options,

        OrderStatuses? status,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = DbSet.Include(o => o.OrderItems);

        if (status is not null)
        {
            query = query.Where(o => o.Status == status);
        }

        return await query.PaginateAsync(options, cancellationToken);
    }

    public async Task<Order?> GetByIdAsync(GetOrderQueryOptions options, CancellationToken cancellationToken = default)
    {
        IQueryable<Order> query = DbSet;
        if (options.IncludeProducts == true)
        {
            query = query.Include(o => o.OrderItems).ThenInclude(oi => oi.Product);
        }
        if (options.IncludeOrderItems == true) query = query.Include(o => o.OrderItems);

        return await query.FirstOrDefaultAsync(o => o.Id == options.Id, cancellationToken);
    }
}