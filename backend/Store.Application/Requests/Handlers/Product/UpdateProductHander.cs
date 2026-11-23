using Application.Common.Errors;
using Domain.Extensions;
using Store.Application.Interfaces;
using Store.Application.Interfaces.Repositories;
using Store.Application.Requests.Commands.Product;

namespace Store.Application.Requests.Handlers.Product;

public class UpdateProductHandler (IProductRepository repository, IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
{
    public async Task<Domain.Models.Product> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var currentUser = currentUserProvider.GetCurrentUser();

        if (!currentUser.Role.IsAdminOrAbove())
        {
            throw new ForbiddenException();
        }

        var product = await repository.GetByIdAsync(request.Id, ct);
        if (product is null)
        {
            throw new NotFoundException("Product not found");
        }
        product.Update(
            request.Name, 
            request.Description, 
            request.Details,
            request.Specs,
            request.Price, 
            request.Quantity,
            request.Category);
        await unitOfWork.SaveChangesAsync(ct);
        return product;
    }
}