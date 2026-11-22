using Application.Common.Errors;
using Store.Application.Interfaces;
using Domain.Extensions;
using Store.Application.Interfaces.Repositories;

namespace Application.Handlers;

public class DeleteProductHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
{
    public async Task Handle(int id, CancellationToken ct)
    {
        var currentUser = currentUserProvider.GetCurrentUser();

        if (!currentUser.Role.IsAdminOrAbove())
        {
            throw new ForbiddenException();
        }


        var anythingDeleted = await productRepository.DeleteByIdAsync(id, ct);
        if (!anythingDeleted)
        {
            throw new NotFoundException();
        }
        await unitOfWork.SaveChangesAsync(ct);
    }
}