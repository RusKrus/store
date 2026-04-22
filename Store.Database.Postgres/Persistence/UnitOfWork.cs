using Store.Application.Interfaces;

namespace Store.Database.Postgres.Persistence;

public class UnitOfWork(StoreContext context): IUnitOfWork
{
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}