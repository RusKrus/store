using Application.Common.Errors;
using Application.Common.Queries;
using Store.Application.Interfaces;
using Store.Application.Interfaces.CartCookiesService;
using Store.Domain.Models;

namespace Application.Handlers.Carts;

public class DeleteProductFromCartCommandHandler(
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork,
    ICartCookiesService cartCookiesService,
    ICurrentUserProvider currentUserProvider)
{
    public async Task Handle(int productId, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        Cart? userCart;
        if (currentUser is null)
        {
            var cartGuid = cartCookiesService.GetCartGuidFromCookies();
            if (cartGuid is null) throw new NotFoundException("User's cart is not found");
            var options = new GetCartByGuidQueryOptions(cartGuid.Value, true, false);
            userCart = await cartRepository.GetByGuidAsync(options, cancellationToken);
        }
        else
        {
            var options = new GetCartByUserQueryOptions(currentUser.Id, true, false);
            userCart = await cartRepository.GetByUserIdAsync(options, cancellationToken);
        }

        if (userCart is null) throw new NotFoundException("User's cart is not found");
        var result = userCart.RemoveCartItem(productId);
        if (!result) throw new NotFoundException("Item is not is not found");

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}