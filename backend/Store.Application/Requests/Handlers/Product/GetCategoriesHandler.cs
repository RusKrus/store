using Store.Application.Interfaces;
using Store.Domain.Enums;

namespace Application.Handlers;

public class GetCategoriesHandler(IProductRepository productRepository)
{
    public async Task<List<ProductCategory>> Handle(bool? showAll, CancellationToken ct)
    {
        return await productRepository.GetAllProductCategoryNames(showAll ?? false, ct);
    }
}