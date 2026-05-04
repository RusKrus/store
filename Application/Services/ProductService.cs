using Application.Common.PaginationOptions;
using Application.Common.PaginationResult;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Services;


public class ProductService(IProductRepository productRepository, IUnitOfWork unitOfWork)
{
    public async Task<PaginationResult<Product>> GetAllProducts(PaginationOptions options, CancellationToken cancellationToken = default)
    {
        return await productRepository.GetPaginatedListAsync(options, cancellationToken);
    }
}