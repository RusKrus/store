using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Database.Postgres.Persistence;
using Store.Domain.Models;

namespace Store.Database.Postgres.Repositories;

public class UserRepository(StoreContext context): IUserRepository
{
    public async Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }

    public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(cancellationToken);
        return user;
    }

    public async Task DeleteByIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        await context.Users.Where(user => user.Id == userId).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task DeleteByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        await context.Users.Where(user => user.Email == email).ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<List<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Users.ToListAsync(cancellationToken);
    }
}