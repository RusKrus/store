using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Domain.Common;
using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Domain.Models;
using Store.Infrastructure.Extensions;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public abstract class BaseRepository<T>(StoreContext context) : IBaseRepository<T> where T : BaseEntity
{
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken: cancellationToken);
    }

    public virtual async Task CreateAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(entity);
        DbSet.Update(entity);
        return await Task.FromResult(entity);
    }

    public virtual async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var deleteCount = await DbSet.Where(p => p.Id == id).ExecuteDeleteAsync(cancellationToken);
        return deleteCount > 0;
    }
}