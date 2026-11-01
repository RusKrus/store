using Application.Common.Errors;
using Application.Common.Queries;
using Store.Application.Interfaces;
using Store.Application.Interfaces.CartCookiesService;
using Store.Domain.Models;

namespace Application.Handlers.Carts;

public class GetCurrentUserCartQueryHandler(
    ICartRepository cartRepository,
    ICartCookiesService cartCookiesService,
    ICurrentUserProvider currentUserProvider)
{
    public async Task<Cart> Handle(CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();

        if (currentUser is null)
        {
            var guid = cartCookiesService.GetCartGuidFromCookies();
            if (guid is null) throw new NotFoundException("Cart is not found");
            var options = new GetCartByGuidQueryOptions(guid.Value, true, true);
            return await cartRepository.GetByGuidAsync(options, cancellationToken)
                   ?? throw new NotFoundException("Cart is not found");
        }
        else
        {
            var options = new GetCartByUserQueryOptions(currentUser.Id, true, true);
            return await cartRepository.GetByUserIdAsync(options, cancellationToken)
                   ?? throw new NotFoundException("Cart is not found");
        }
    }
}