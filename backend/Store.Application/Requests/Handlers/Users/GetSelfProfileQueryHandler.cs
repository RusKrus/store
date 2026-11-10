using Application.Common.Errors;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Users;

public class GetSelfProfileQueryHandler(IUserRepository repository, ICurrentUserProvider provider)
{
    public async Task<User> Handle(CancellationToken cancellationToken)
    {
        var currentUser =  provider.GetCurrentUser();

        var user = await repository.GetByIdAsync(currentUser.Id, cancellationToken);

        return user ?? throw new NotFoundException();
    }


}