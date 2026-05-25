using System.ComponentModel.DataAnnotations;
using Application.Commands.Users;
using Application.Handlers.Users;
using Application.Queiries.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.API.Contracts.Requests.Common;
using Store.API.Contracts.Requests.User;
using Store.API.Contracts.Response;
using Store.API.Contracts.Response.Users;
using Store.API.Extensions;
using Store.Domain.Enums;

namespace Store.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IMapper mapper)
{
    /// <summary>
    ///     Returns all existing users paginated
    /// </summary>
    /// <param name="request"></param>
    /// <param name="searchString"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<ActionResult<PaginationResponse<UserResponse>>> GetAllUsers(
        PaginationRequest request,
        string? searchString,
        GetAllUsersQueryHandler handler,
        CancellationToken ct)
    {
        var paginationOptions = request.ToOptions();
        var query = new GetAllUsersQuery(paginationOptions, searchString);
        var result = await handler.Handle(query, ct);
        var response = mapper.Map<PaginationResponse<UserResponse>>(result);
        return response;
    }

    /// <summary>
    ///     Returns user, found by email
    /// </summary>
    /// <param name="email"></param>
    /// <returns></returns>
    [HttpGet("by-email")]
    public async Task<ActionResult<UserResponse>> GetUserByEmail([EmailAddress][FromQuery] string email, GetUserByEmailHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(email, ct);
        var response = mapper.Map<UserResponse>(result);
        return response;
    }

    /// <summary>
    ///     Returns user, found by email
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetUserById(int id, GetUserByIdHandler handler, CancellationToken ct)
    {
        var result = await handler.Handle(id, ct);
        var response = mapper.Map<UserResponse>(result);
        return response;
    }

    /// <summary>
    ///     Creates user
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize(Roles = nameof(UserRole.Admin))]
    public async Task<ActionResult<int>> CreateUser([FromBody] CreateUserRequest request, CreateUserCommandHandler handler, CancellationToken ct)
    {
        var command = mapper.Map<CreateUserCommand>(request);
        return await handler.Handle(command, ct);
    }
}