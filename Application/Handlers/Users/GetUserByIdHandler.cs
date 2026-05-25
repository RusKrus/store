using Application.Common.Errors;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Users;

public class GetUserByIdHandler(IUserRepository repository)
{
    public async Task<User> Handle(int id, CancellationToken ct)
    {
        return await repository.GetByIdAsync(id, ct) ?? throw new NotFoundException("User not found");
    }
}