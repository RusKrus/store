using Microsoft.AspNetCore.Mvc;
using Application.Services;

namespace Store.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController(ProductService productService)
{

    // public Task<ActionResult> GetAllProducts(CancellationToken ct)
    // {
    //     return
    // }
}