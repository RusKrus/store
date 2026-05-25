using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Microsoft.EntityFrameworkCore;

namespace Store.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginationResult<T>> PaginateAsync<T>(
        this IQueryable<T> list,
        PaginationOptions options,
        CancellationToken cancellationToken = default)
    {
        return new PaginationResult<T> {
            Total = list.Count(),
            Items = await list.Skip(options.Offset).Take(options.Limit).ToListAsync(cancellationToken)
        };
    }
}