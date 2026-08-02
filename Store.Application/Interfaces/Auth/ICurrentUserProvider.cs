using Application.Auth;

namespace Store.Application.Interfaces;

public interface ICurrentUserProvider
{
    CurrentUserModel? GetCurrentUser();
}