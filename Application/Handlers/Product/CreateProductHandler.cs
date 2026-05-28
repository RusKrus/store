using Application.Commands;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers;

public class CreateProductHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct = default)
    {
        var product = new Product(request.Name, request.Description, request.Price, request.Quantity);
        await productRepository.CreateAsync(product, ct);
        await unitOfWork.SaveChangesAsync(ct);
        return product.Id;
    }
}