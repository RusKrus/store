using Application.Commands.Auth;
using Application.Handlers.Auth;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.API.Contracts.Requests.Auth;

namespace Store.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorizationController(IMapper mapper)
{
    [HttpPost("/register")]
    public async Task<int> Register([FromBody] RegisterRequest request, RegisterCommandHandler handler, CancellationToken ct)
    {
        var command = mapper.Map<RegisterCommand>(request);
        var createdUserId = await handler.Handle(command, ct);
        return createdUserId;
    }

    [HttpPost("/login")]
    public async Task<string> Login([FromBody] LoginRequest request, LoginCommandHandler handler, CancellationToken ct)
    {
        var command = mapper.Map<LoginCommand>(request);
        var token = await handler.Handle(command, ct);
        return token;
    }
}