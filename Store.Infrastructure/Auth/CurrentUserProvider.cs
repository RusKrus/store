using System.Security.Claims;
using Application.Auth;
using Application.Common.Errors;
using Store.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Store.Domain.Enums;

namespace Store.Infrastructure.Auth;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    public CurrentUserModel GetCurrentUser()
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user is null)
        {
            throw new UnauthorizedException("User not authorized");
        }
        return new CurrentUserModel(
            int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)),
            user.FindFirstValue(ClaimTypes.Email),
            user.FindFirstValue(ClaimTypes.Name),
            Enum.Parse<UserRole>(user.FindFirstValue(ClaimTypes.Role))
            );
    }
}