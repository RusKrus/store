using Application.Common.Errors;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Users;

public class GetUserByEmailHandler(IUserRepository repository)
{
    public async Task<User> Handle(string email, CancellationToken ct)
    {
        var user = await repository.GetByEmailAsync(email, ct);
        if (user == null)
        {
            throw new NotFoundException($"User with email {email} not found");
        }
        return user;
    }
}