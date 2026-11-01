using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Store.Domain.Enums;

namespace Tests.Integration.Fakes;

public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemaName = "Test";

    public const string UserIdHeader = "X-Test-User-Id";
    public const string EmailHeader = "X-Test-User-Email";
    public const string NameHeader = "X-Test-User-Name";
    public const string RoleHeader = "X-Test-User-Role";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var authorization)
            || authorization.Scheme != SchemaName)
        {
            return Task.FromResult(AuthenticateResult.Fail("Error parsing test authentication."));
        }

        var userId = Request.Headers[UserIdHeader].ToString();
        var userEmail = Request.Headers[EmailHeader].ToString();
        var name = Request.Headers[NameHeader].ToString();
        Enum.TryParse<UserRole>(Request.Headers[RoleHeader], out var role);
        var roleString = role.ToString();

        if (string.IsNullOrWhiteSpace(roleString) ||
            string.IsNullOrWhiteSpace(userId) ||
            string.IsNullOrWhiteSpace(userEmail) ||
            string.IsNullOrWhiteSpace(name))
        {
            return Task.FromResult(AuthenticateResult.Fail("Failed to parse user credentials"));
        }

        Claim[] claims =
        [
            new (ClaimTypes.NameIdentifier, userId),
            new (ClaimTypes.Email, userEmail),
            new (ClaimTypes.Role, roleString),
            new (ClaimTypes.Name, name)
        ];

        var identity = new ClaimsIdentity(claims, SchemaName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemaName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}