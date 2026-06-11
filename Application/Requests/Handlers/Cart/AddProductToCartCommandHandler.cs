using Application.Commands.Cart;
using Application.Common.Errors;
using Application.Common.Queries;
using Store.Application.Interfaces;
using Store.Application.Interfaces.CartCookiesService;
using Store.Domain.Models;

namespace Application.Handlers.Carts;

public class AddProductToCartCommandHandler(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ICartCookiesService cartCookiesService,
    ICurrentUserProvider currentUserProvider)
{
    public async Task Handle(
        AddProductToCartCommand command,
        CancellationToken cancellationToken
        )
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        Cart? userCart;

        if (currentUser is null)
        {
            var cartGuid = cartCookiesService.GetCartGuidFromCookies();
            if (cartGuid is null)
            {
                userCart = new Cart(null);
                cartGuid = userCart.CartGuid;
                cartCookiesService.SaveCartInCookies(cartGuid.Value);
                await cartRepository.CreateAsync(userCart, cancellationToken);
            }
            else
            {
                var options = new GetCartByGuidQueryOptions(cartGuid.Value, true, true);
                userCart = await cartRepository.GetByGuidAsync(options, cancellationToken);
                if (userCart is null)
                {
                    cartCookiesService.DeleteCartFromCookies();
                    userCart = new Cart(null);
                    cartGuid = userCart.CartGuid;
                    cartCookiesService.SaveCartInCookies(cartGuid.Value);
                    await cartRepository.CreateAsync(userCart, cancellationToken);
                }
            }
        }
        else
        {
            var options = new GetCartByUserQueryOptions(currentUser.Id, true, true);
            userCart = await cartRepository.GetByUserIdAsync(options, cancellationToken);
            if (userCart is null)
            {
                userCart = new Cart(currentUser.Id);
                await cartRepository.CreateAsync(userCart, cancellationToken);
            }
        }

        var product = await productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product is null) throw new NotFoundException("Product not found");

        if (command.Quantity > product.Quantity) throw new ValidationException($"Too much quantity requested. Only {product.Quantity} items available.");

        userCart.AddProduct(command.ProductId, command.Quantity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

//     private Task CreateAndSaveAnonymousCartAsync()
//     {
//         userCart = new Cart(null);
//         cartGuid = userCart.CartGuid;
//         cartCookiesService.SaveCartInCookies(cartGuid.Value);
//         await cartRepository.CreateAsync(userCart, cancellationToken);
//     }
}