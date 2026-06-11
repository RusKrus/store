using Application.Common.Queries;
using Store.Domain.Models;

namespace Store.Application.Interfaces;

public interface ICartRepository : IBaseRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(
        GetCartByUserQueryOptions options,
        CancellationToken cancellationToken = default);

    Task<Cart?> GetByGuidAsync(
        GetCartByGuidQueryOptions options,
        CancellationToken cancellationToken = default);
}