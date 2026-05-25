using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Domain.Models;
using Store.Infrastructure.Extensions;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public class UserRepository(StoreContext context): IUserRepository
{
    public async Task CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }
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

    public async Task<PaginationResult<User>> GetPaginatedListAsync(PaginationOptions options, string? searchString, CancellationToken cancellationToken = default)
    {
        IQueryable<User> query = context.Users;
        if (searchString != null)
        {
            query = query.Where(u => u.Email.Contains(searchString) || u.FirstName.Contains(searchString) || u.LastName.Contains(searchString));
        }
        return await query.PaginateAsync(options, cancellationToken);
    }
}