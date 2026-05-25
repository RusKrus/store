using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers;

public class GetProductByIdHandler(IProductRepository repository, IUnitOfWork unitOfWork)
{
    public async Task<Product?> Handle(int id, CancellationToken ct)
    {
        return await repository.GetByIdAsync(id, ct);
    }
}