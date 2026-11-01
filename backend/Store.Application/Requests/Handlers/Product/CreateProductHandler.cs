using Application.Commands;
using Application.Common.Errors;
using Domain.Extensions;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers;

public class CreateProductHandler(IProductRepository productRepository, IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct = default)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        if (currentUser is null) throw new UnauthorizedException();
        if (!currentUser.Role.IsAdminOrAbove()) throw new ForbiddenException();

        var product = new Product(request.Name, request.Description, request.Price, request.Quantity);
        await productRepository.CreateAsync(product, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return product.Id;
    }
}