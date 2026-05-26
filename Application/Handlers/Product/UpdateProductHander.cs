using Application.Commands;
using Application.Common.Errors;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers;

public class UpdateProductHandler (IProductRepository repository, IUnitOfWork unitOfWork)
{
    public async Task<Product> Handle(UpdateProductCommand request, CancellationToken ct)
    {
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