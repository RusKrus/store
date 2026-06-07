using Application.Common.Errors;
using Application.Common.Queries;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Carts;

public class GetCurrentUserCartQueryHandler(
    ICartRepository cartRepository,
    ICurrentUserProvider currentUserProvider)
{
    public async Task<Cart> Handle(CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        if (currentUser is null) throw new UnauthorizedException();

        var options = new GetCartQueryOptions(currentUser.Id, true, true);
        var currentUserCart = await cartRepository.GetByUserIdAsync(options, cancellationToken);

        return currentUserCart ?? throw new NotFoundException("Cart is not found");
    }
}