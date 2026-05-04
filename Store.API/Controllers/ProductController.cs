using Microsoft.AspNetCore.Mvc;
using Application.Services;
using Store.API.Contracts.Requests.Common;
using Store.API.Contracts.Response;
using Store.API.Extensions;
using Store.Domain.Models;

namespace Store.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ProductService productService) : ControllerBase
{

    /// <summary>
    ///     Returns all available products in store, paginated
    /// </summary>
    /// <param></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<PaginationResponse<Product>> GetProducts(PaginationRequest request, string? SearchString, CancellationToken ct)
    {
        var paginationOptions = request.ToOptions();
        var result = await productService.GetAllProducts(paginationOptions, ct);
        return new PaginationResponse<Product>(result.Total, result.Items);
    }
}