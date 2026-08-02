using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Store.Domain.Models;


namespace Store.Application.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<PaginationResult<User>> GetPaginatedListAsync(PaginationOptions options, string? searchString, CancellationToken cancellationToken = default);
}