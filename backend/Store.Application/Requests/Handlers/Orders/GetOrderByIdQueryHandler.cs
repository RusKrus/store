using Application.Common.Errors;
using Application.Common.Queries;
using Domain.Extensions;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Orders;

public class GetOrderByIdQueryHandler(IOrderRepository repository, ICurrentUserProvider currentUserProvider)
{
    public async Task<Order> Handle(int id, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        if (currentUser is null) throw new UnauthorizedException();

        var options = new GetOrderQueryOptions(id, true, false);
        var order = await repository.GetByIdAsync(options, cancellationToken);
        if (order is null) throw new NotFoundException();

        var orderUserId = order.UserId;
        if (orderUserId != currentUser.Id)
        {
            if (currentUser.Role.IsAdminOrAbove()) return order;
            throw new ForbiddenException();
        }

        return order;
    }
}