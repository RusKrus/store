using Application.Common.Errors;
using Domain.Extensions;
using Store.Application.Interfaces;

namespace Application.Handlers.Users;

public class DeleteUserHandler(IUserRepository repository, IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
{
  public async Task Handle(int id, CancellationToken ct)
  {
    var currentUser = currentUserProvider.GetCurrentUser();
    var userToDelete = await repository.GetByIdAsync(id, ct);
    if (userToDelete is null)
    {
      return;
    }

    if (!currentUser.Role.HasGreaterRole(userToDelete.Role))
    {
      throw new ForbiddenException("This user can't be deleted");
    }

    await repository.DeleteByIdAsync(id, ct);
    await unitOfWork.SaveChangesAsync(ct);
  }
}