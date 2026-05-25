using Application.Commands.Auth;
using Application.Common.Errors;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Auth;

public class RegisterCommandHandler(IUserRepository repository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
{
    public async Task<int> Handle(RegisterCommand command, CancellationToken ct)
    {
        var existingUser = await repository.GetByEmailAsync(command.Email, ct);
        if (existingUser != null)
        {
            throw new ConflictException("User with such email already exists");
        }

        var passwordHashed = passwordHasher.Hash(command.Password);
        var user = new User(command.FirstName, command.LastName, command.Email, passwordHashed, null);
        repository.CreateAsync(user, ct);
        unitOfWork.SaveChangesAsync(ct);
        return user.Id;
    }
}