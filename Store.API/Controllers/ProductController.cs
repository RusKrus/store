using Application.Commands;
using Application.Handlers;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Contracts.Requests.Common;
using Store.API.Contracts.Requests.Product;
using Store.API.Contracts.Response;
using Store.API.Extensions;
using Store.Domain.Enums;
using Store.Domain.Models;

namespace Store.Api.Controllers;

/// <summary>
///     General actions under products in store
/// </summary>
/// <param name="productService"></param>
[ApiController]
[Route("api/[controller]")]
public class ProductController(IMapper mapper) : ControllerBase
{
    /// <summary>
    ///     Returns all available products in store, paginated
    /// </summary>
    /// <param name="request"></param>
    /// <param name="searchString"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<PaginationResponse<ProductResponse>> GetProducts(
        PaginationRequest request,
        string? searchString,
        GetPaginatedProductsHandler handler,
        CancellationToken ct)
    {
        var paginationOptions = request.ToOptions();
        var query = new GetPaginatedProductsQuery(paginationOptions, searchString);
        var result = await handler.Handle(query, ct);
        var response = mapper.Map<PaginationResponse<ProductResponse>>(result);
        return response;
    }

    /// <summary>
    ///     Returns specified product
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProductResponse>> GetProductById(int id, GetProductByIdHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(id, ct);
        if (result == null)
        {
            return NotFound();
        }

        var response = mapper.Map<ProductResponse>(result);
        return Ok(response);
    }

    /// <summary>
    ///     Delete specified product
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [Authorize(Roles = nameof(UserRole.Admin))]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> DeleteProductById(int id, DeleteProductHandler deleteProductHandler, CancellationToken ct)
    {
        await deleteProductHandler.Handle(id, ct);
        return NoContent();
    }

    /// <summary>
    ///     Adds product to the list of products
    /// </summary>
    /// <param name="productToCreate"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<int>> CreateProductAsync(
        [FromBody] CreateProductRequest productToCreate,
        CreateProductHandler productRequestHandler,
        CancellationToken ct)
    {
        var createProductCommand = mapper.Map<CreateProductCommand>(productToCreate);

        var id = await productRequestHandler.Handle(createProductCommand, ct);
        return CreatedAtAction(nameof(GetProductById), new { id }, new { id });
    }

    /// <summary>
    ///     Updates selected product
    /// </summary>
    /// <param name="newProductData"></param>
    /// <returns></returns>
    [HttpPut("{id:int}")]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<ProductResponse>> UpdateProduct(
        [FromBody] UpdateProductRequest newProductData,
        [FromRoute] int id,
        UpdateProductHandler handler,
        CancellationToken ct)
    {
        var command = new UpdateProductCommand(
            id,
            newProductData.Name,
            newProductData.Description,
            newProductData.Price,
            newProductData.Quantity
        );
        var result = await handler.Handle(command, ct);
        var response = mapper.Map<ProductResponse>(result);
        return response;
    }
}