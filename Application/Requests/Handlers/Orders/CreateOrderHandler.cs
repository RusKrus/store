using Application.Common.Errors;
using Application.Common.Queries;
using Application.Requests.Commands.Orders;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Orders;

public class CreateOrderHandler(
    IOrderRepository orderRepository,
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserProvider currentUserProvider)
{
    public async Task<int> Handle(CreateOrderCommand command, CancellationToken ct)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        if (currentUser is null) throw new UnauthorizedException();

        var getCartOptions = new GetCartQueryOptions(currentUser.Id, true, true);
        var currentUserCart = await cartRepository.GetByUserIdAsync(getCartOptions, ct);
        if (currentUserCart is null) throw new NotFoundException("User's cart is not found");

        if (currentUserCart.CartItems.Count == 0) throw new ValidationException("Cart is empty");

        var receiverInfo = new OrderReceiverInfo(
            command.ReceiverFirstName,
            command.ReceiverLastName,
            command.ReceiverAddress,
            command.ReceiverEmail,
            command.ReceiverPhoneNumber,
            command.City,
            command.OrderComment
            );
        var order = new Order(
            currentUser.Id,
            currentUserCart.TotalPrice,
            DateTime.UtcNow.AddDays(Random.Shared.Next(1, 3)),
            receiverInfo
            );

        foreach (var cartItem in currentUserCart.CartItems)
        {
            order.AddOrderItem(cartItem.ProductId, order.Id, cartItem.Product.Price, cartItem.Quantity);
        }

        await orderRepository.CreateAsync(order, ct);
        currentUserCart.ClearCart();
        await unitOfWork.SaveChangesAsync(ct);

        return order.Id;
    }
}