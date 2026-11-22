using Application.Common.Errors;
using Domain.Extensions;
using Store.Application.Interfaces;
using Store.Application.Interfaces.Repositories;
using Store.Application.Requests.Commands.Product;

namespace Store.Application.Requests.Handlers.Product;

public class CreateProductHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct = default)
    {
        var currentUser = currentUserProvider.GetCurrentUser();

        if (!currentUser.Role.IsAdminOrAbove()) throw new ForbiddenException();

        var product = new Domain.Models.Product(
            request.Name, 
            request.Description, 
            request.Details,
            request.Specs,
            request.Price, 
            request.Quantity,
            request.Category);
        await productRepository.CreateAsync(product, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return product.Id;
    }
}