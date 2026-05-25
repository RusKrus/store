using Store.Application.Interfaces;

namespace Application.Handlers;

public class DeleteProductHandler(IProductRepository productRepository, IUnitOfWork unitOfWork)
{
    public async Task Handle(int id, CancellationToken ct)
    {
        await productRepository.DeleteByIdAsync(id, ct);
        await unitOfWork.SaveChangesAsync(ct);
    }
}