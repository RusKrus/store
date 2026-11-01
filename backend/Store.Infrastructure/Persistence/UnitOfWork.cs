using Microsoft.EntityFrameworkCore.Storage;
using Store.Application.Interfaces;

namespace Store.Infrastructure.Persistence;

public class UnitOfWork(StoreContext context): IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken)
    {
        return await context.Database.BeginTransactionAsync(cancellationToken);
    }
}