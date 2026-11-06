using Application.Commands.Users;
using Application.Common.Errors;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Users;

public class UpdateProfileHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider)
{
    public async Task<User> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();

        var userToUpdate = await repository.GetByIdAsync(currentUser.Id, cancellationToken);
        if (userToUpdate is null)
        {
            throw new NotFoundException("User not found");
        }

        userToUpdate.UpdateProfile(command.FirstName, command.LastName, command.Email);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        return userToUpdate;
    }
}