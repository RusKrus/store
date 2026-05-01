using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Services;


public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
{
    public async Task<List<Product>> GetAllProducts(CancellationToken cancellationToken = default)
    {
        return await productRepository.GetAllAsync(cancellationToken);
    }
}