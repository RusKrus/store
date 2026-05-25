using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Store.Domain.Models;


namespace Store.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteByIdAsync(int userId, CancellationToken cancellationToken = default);
    Task DeleteByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<PaginationResult<User>> GetPaginatedListAsync(PaginationOptions options, string? searchString, CancellationToken cancellationToken = default);
    Task CreateAsync(User user, CancellationToken cancellationToken = default);
}