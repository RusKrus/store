using Application.Commands.Users;
using Application.Common.Errors;
using Store.Application.Interfaces;

namespace Application.Handlers.Users;

public class UpdatePasswordHandler(
    IUserRepository repository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher,
    ICurrentUserProvider currentUserProvider)
{
    public async Task Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        if (currentUser == null)
        {
            throw new UnauthorizedException();
        }

        var userToUpdate = await repository.GetByIdAsync(currentUser.Id, cancellationToken);
        if (userToUpdate == null)
        {
            throw new UnauthorizedException("User not found");
        }

        var isOldPasswordValid = passwordHasher.Verify(userToUpdate.PasswordHash, request.OldPassword);
        if (!isOldPasswordValid)
        {
            throw new ValidationException("Password doesn't match");
        }

        var newPasswordHash = passwordHasher.Hash(request.NewPassword);
        userToUpdate.UpdatePassword(newPasswordHash);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}