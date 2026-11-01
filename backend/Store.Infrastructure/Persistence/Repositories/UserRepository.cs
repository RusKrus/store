using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Microsoft.EntityFrameworkCore;
using Store.Application.Interfaces;
using Store.Domain.Models;
using Store.Infrastructure.Extensions;
using Store.Infrastructure.Persistence;

namespace Store.Infrastructure.Repositories;

public class UserRepository(StoreContext context) : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await context.Users
            .Include(u => u.Cart)
            .ThenInclude(c => c.CartItems)
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
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