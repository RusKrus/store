using Application.Common.Errors;
using Application.Common.Queries;
using Store.Application.Interfaces;

namespace Application.Handlers.Carts;

public class DeleteProductFromCartCommandHandler(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider)
{
    public async Task Handle(int productId, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        if (currentUser is null)
        {
            throw new UnauthorizedException();
        }

        var options = new GetCartQueryOptions(currentUser.Id, true, false);
        var userCart = await cartRepository.GetByUserIdAsync(options, cancellationToken);
        if (userCart is null) throw new NotFoundException("User's cart is not found");

        var result = userCart.RemoveCartItem(productId);
        if (!result) throw new NotFoundException("User's cart is not found");

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}