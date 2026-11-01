using Application.Commands.Users;
using Application.Common.Errors;
using Domain.Extensions;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Users;

public class CreateUserCommandHandler(IUserRepository repository, IUnitOfWork unitOfWork, IPasswordHasher passwordHasher, ICurrentUserProvider currentUserProvider)
{
    public async Task<int> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        if (!currentUser.Role.HasGreaterRole(command.Role))
        {
            throw new ForbiddenException("You are unable to do this");
        }

        var hashedPassword = passwordHasher.Hash(command.Password);
        var newUser = new User(command.FirstName, command.LastName, command.Email, hashedPassword, command.Role);
        await repository.CreateAsync(newUser, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return newUser.Id;
    }
}