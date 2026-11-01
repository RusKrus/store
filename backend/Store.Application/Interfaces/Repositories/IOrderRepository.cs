using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Application.Common.Queries;
using Store.Domain.Enums;
using Store.Domain.Models;

namespace Store.Application.Interfaces;

public interface IOrderRepository : IBaseRepository<Order>
{
    Task<PaginationResult<Order>> GetPaginatedListAsync(
        PaginationOptions options,
        OrderStatuses? status,
        CancellationToken cancellationToken = default);

    Task<Order?> GetByIdAsync(GetOrderQueryOptions options, CancellationToken cancellationToken = default);
}