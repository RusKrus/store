using Application.Handlers.Orders;
using Application.Requests.Commands.Orders;
using Application.Requests.Queiries.Orders;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Contracts.Requests.Common;
using Store.API.Contracts.Requests.Order;
using Store.API.Contracts.Response;
using Store.API.Contracts.Response.Orders;
using Store.API.Extensions;
using Store.Domain.Enums;

namespace Store.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IMapper mapper) : ControllerBase
{
    /// <summary>
    ///     Creates order from the user cart. Requires user data
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<int>> CreateOrder(CreateOrderRequest request, CreateOrderHandler handler, CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateOrderCommand>(request);
        return await handler.Handle(command, cancellationToken);
    }

    /// <summary>
    ///     Returns current user orders, paginated
    /// </summary>
    /// <param name="request"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    [HttpGet("self-orders")]
    [Authorize]
    public async Task<ActionResult<PaginationResponse<OrderResponse>>> GetUserOrders(
        PaginationRequest request,
        OrderStatuses? status,
        GetUserOrdersQueryHandler handler,
        CancellationToken ct)
    {
        var options = request.ToOptions();
        var query = new GetUserOrdersQuery(options, status);
        var paginatedOrders = await handler.Handle(query, ct);
        var paginatedResponse = mapper.Map<PaginationResponse<OrderResponse>>(paginatedOrders);
        return paginatedResponse;
    }

    /// <summary>
    ///     Returns single order. Only admins allowed seeing another user's orders
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<ActionResult<OrderResponse>> GetOrderById(
        int id,
        GetOrderByIdQueryHandler handler,
        CancellationToken ct
    )
    {
        var order = await handler.Handle(id, ct);
        var response = mapper.Map<OrderResponse>(order);
        return response;
    }

    [HttpPost("cancel-order")]
    [Authorize]
    public async Task<bool> CancelOrder(int orderId, CancelOrderCommandHandler handler, CancellationToken ct)
    {
        await handler.Handle(orderId, ct);
        return true;
    }
}