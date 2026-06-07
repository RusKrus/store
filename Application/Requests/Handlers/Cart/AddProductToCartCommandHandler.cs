using Application.Commands.Cart;
using Application.Common.Errors;
using Application.Common.Queries;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Carts;

public class AddProductToCartCommandHandler(
    ICartRepository cartRepository,
    IProductRepository productRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider)
{
    public async Task Handle(AddProductToCartCommand command, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        if (currentUser is null) throw new UnauthorizedException();

        var options = new GetCartQueryOptions(currentUser.Id, true, true);
        var userCart = await cartRepository.GetByUserIdAsync(options, cancellationToken);
        if (userCart is null)
        {
            userCart = new Cart(currentUser.Id);
            await cartRepository.CreateAsync(userCart, cancellationToken);
        }

        var product = await productRepository.GetByIdAsync(command.ProductId, cancellationToken);
        if (product is null) throw new NotFoundException("Product not found");

        if (command.Quantity > product.Quantity) throw new ValidationException($"Too much quantity requested. Only {product.Quantity} items available.");

        var cartItem = userCart!.CartItems.FirstOrDefault(ci => ci.ProductId == command.ProductId);
        userCart.AddProduct(command.ProductId, command.Quantity);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}