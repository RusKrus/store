using System.ComponentModel.DataAnnotations;
using Application.Commands.Cart;
using Application.Handlers.Carts;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Contracts.Requests.Cart;
using Store.API.Contracts.Response.Cart;

namespace Store.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController(IMapper mapper) : ControllerBase
{
    /// <summary>
    ///     Adds product to cart
    /// </summary>
    /// <remarks>
    ///     Number of products should be greater than 0, else use delete product from cart endpoint
    /// </remarks>
    /// <param name="request"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("product")]
    public async Task<ActionResult> AddProductToCart(
        [FromBody] AddProductToCartRequest request,
        AddProductToCartCommandHandler handler,
        CancellationToken cancellationToken)
    {
        var command = mapper.Map<AddProductToCartCommand>(request);
        await handler.Handle(command, cancellationToken);
        return Ok();
    }

    /// <summary>
    ///     Removes product from cart
    /// </summary>
    /// <param name="productId"></param>
    /// <returns></returns>
    [Authorize]
    [HttpDelete("product")]
    public async Task<ActionResult> DeleteProductFromCart(
        [FromBody] int productId,
        DeleteProductFromCartCommandHandler handler,
        CancellationToken cancellationToken)
    {
        await handler.Handle(productId, cancellationToken);
        return NoContent();
    }

    [Authorize]
    [HttpGet("self-cart")]
    public async Task<ActionResult<CartResponse>> GetCurrentUserCart(
        GetCurrentUserCartQueryHandler handler,
        CancellationToken cancellationToken)
    {
        var cart = await handler.Handle(cancellationToken);
        return mapper.Map<CartResponse>(cart);
    }
}