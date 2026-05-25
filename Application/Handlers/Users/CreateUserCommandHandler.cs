using Application.Commands.Users;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Users;

public class CreateUserCommandHandler(IUserRepository repository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
{
    public async Task<int> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var hashedPassword = passwordHasher.Hash(command.Password);
        var newUser = new User(command.FirstName, command.LastName, command.Email, hashedPassword, command.Role);
        await repository.CreateAsync(newUser, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return newUser.Id;
    }
}