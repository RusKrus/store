using Application.Common.PaginationResult;
using Application.Queiries.Users;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Users;

public class GetAllUsersQueryHandler(IUserRepository repository)
{
    public async Task<PaginationResult<User>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return await repository.GetPaginatedListAsync(request.Options, request.SearchString, cancellationToken);
    }
}