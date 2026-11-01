using Microsoft.AspNetCore.Http;
using Store.Application.Interfaces.CartCookiesService;

namespace Store.Infrastructure.Services;

public class CartCookiesService(IHttpContextAccessor contextAccessor) : ICartCookiesService
{
    public void SaveCartInCookies(Guid cartGuid)
    {
        var context = contextAccessor.HttpContext;
        if (context is null) throw new InvalidOperationException();

        context.Response.Cookies.Append(
            "cartId",
            cartGuid.ToString(),
            new CookieOptions { HttpOnly = true}
            );
    }

    public Guid? GetCartGuidFromCookies()
    {
        var context = contextAccessor.HttpContext;
        if (context is null) throw new InvalidOperationException();

        if (!context.Request.Cookies.TryGetValue("cartId", out var cartGuid))
        {
            return null;
        }
        return Guid.TryParse(cartGuid, out var result) ? result : null;
    }
    public void DeleteCartFromCookies()
    {
        var context = contextAccessor.HttpContext;
        if (context is null) throw new InvalidOperationException();

        context.Response.Cookies.Delete("cartId");
    }
}