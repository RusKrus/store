using Application.Commands.Users;
using Application.Common.Errors;
using Domain.Extensions;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Users;

public class UpdateUserHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ICurrentUserProvider userProvider)
{
    public async Task<User> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var currentUser = userProvider.GetCurrentUser();
        var userToUpdate = await repository.GetByIdAsync(command.Id, cancellationToken);
        if (userToUpdate is null)
        {
            throw new NotFoundException("User not found");
        }

        if (!currentUser.Role.HasGreaterRole(userToUpdate.Role))
        {
            throw new ForbiddenException("You can't update this user");
        }
        if (!currentUser.Role.HasGreaterRole(command.Role))
        {
            throw new ForbiddenException("You can't make such modifications");
        }

        var hashedPassword = passwordHasher.Hash(command.Password);
        userToUpdate.Update(command.FirstName, command.LastName, command.Email,  hashedPassword, command.Role);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return userToUpdate;
    }
}