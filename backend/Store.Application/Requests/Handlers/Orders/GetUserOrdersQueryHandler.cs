using Application.Common.Errors;
using Application.Common.PaginationResult;
using Application.Requests.Queiries.Orders;
using Store.Application.Interfaces;
using Store.Domain.Models;

namespace Application.Handlers.Orders;

public class GetUserOrdersQueryHandler(
    IOrderRepository repository,
    ICurrentUserProvider currentUserProvider
    )
{
    public async Task<PaginationResult<Order>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
    {
        var currentUser = currentUserProvider.GetCurrentUser();
        if (currentUser is null) throw new UnauthorizedException();

        return await repository.GetPaginatedListAsync(request.options, request.status, cancellationToken);
    }
}