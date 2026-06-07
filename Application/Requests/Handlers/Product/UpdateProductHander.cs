using Application.Commands;
using Application.Common.Errors;
using Domain.Extensions;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers;

public class UpdateProductHandler (IProductRepository repository, IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
{
    public async Task<Product> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        if (currentUser is null)
        {
            throw new UnauthorizedException();
        }

        if (!currentUser.Role.IsAdminOrAbove())
        {
            throw new ForbiddenException();
        }

        var product = await repository.GetByIdAsync(request.Id, ct);
        if (product is null)
        {
            throw new NotFoundException("Product not found");
        }
        product.Update(request.Name, request.Description, request.Price, request.Quantity);
        await unitOfWork.SaveChangesAsync(ct);
        return product;
    }
}