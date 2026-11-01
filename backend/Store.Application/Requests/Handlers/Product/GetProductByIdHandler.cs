using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers;

public class GetProductByIdHandler(IProductRepository repository)
{
    public async Task<Product?> Handle(int id, CancellationToken ct)
    {
        return await repository.GetByIdAsync(id, ct);
    }
}