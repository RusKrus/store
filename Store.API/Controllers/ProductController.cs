using Microsoft.AspNetCore.Mvc;
using Application.Services;

namespace Store.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ProductService productService) : ControllerBase
{

    /// <summary>
    ///     Returns all available products in store
    /// </summary>
    /// <param></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult> GetAllProducts(CancellationToken ct)
    {
        var result = await productService.GetAllProducts(ct);
        return Ok(result);
    }
}