using Application.Common.Queries;
using Store.Domain.Models;

namespace Store.Application.Interfaces;

public interface ICartRepository : IBaseRepository<Cart>
{
    Task<Cart?> GetByUserIdAsync(
        GetCartQueryOptions options,
        CancellationToken cancellationToken = default);
}