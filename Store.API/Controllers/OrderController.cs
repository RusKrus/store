using Application.Handlers.Orders;
using Application.Requests.Commands.Orders;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Contracts.Requests.Order;

namespace Store.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController(IMapper mapper) : ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<int>> CreateOrder(CreateOrderRequest request, CreateOrderHandler handler, CancellationToken cancellationToken)
    {
        var command = mapper.Map<CreateOrderCommand>(request);
        return await handler.Handle(command, cancellationToken);
    }
}