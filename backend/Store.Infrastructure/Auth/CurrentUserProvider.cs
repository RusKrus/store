using System.Security.Claims;
using Application.Auth;
using Application.Common.Errors;
using Store.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Store.Domain.Enums;

namespace Store.Infrastructure.Auth;

public class CurrentUserProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    public CurrentUserModel? GetCurrentUser()
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user is null) return null;

        var isIdValid = int.TryParse(user.FindFirstValue(ClaimTypes.NameIdentifier), out var userIdClaim);
        var email = user.FindFirstValue(ClaimTypes.Email);
        var name = user.FindFirstValue(ClaimTypes.Name);
        var isRoleValid = Enum.TryParse<UserRole>(user.FindFirstValue(ClaimTypes.Role), out var roleClaim);

        if (!isIdValid || email is null || name is null || !isRoleValid)
        {
            return null;
        }

        return new CurrentUserModel(
            userIdClaim,
            email,
            name,
            roleClaim
            );
    }
}