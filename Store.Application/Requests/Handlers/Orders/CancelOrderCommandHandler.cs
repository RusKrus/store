using Application.Common.Errors;
using Domain.Extensions;
using Store.Application.Interfaces;

namespace Application.Handlers.Orders;

public class CancelOrderCommandHandler(IOrderRepository repository, IUnitOfWork unitOfWork, ICurrentUserProvider currentUserProvider)
{
    public async Task Handle(int id, CancellationToken ct)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        if (currentUser is null) throw new UnauthorizedException();

        var order = await repository.GetByIdAsync(id, ct);
        if (order is null) throw new NotFoundException();

        if (order.UserId != currentUser.Id)
        {
            if (currentUser.Role.IsAdminOrAbove()) order.CancelOrder();
            throw new ForbiddenException();
        }
        order.CancelOrder();
        await unitOfWork.SaveChangesAsync(ct);
    }
}