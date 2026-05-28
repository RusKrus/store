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
using Store.Domain.Models;

namespace Store.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController(IMapper mapper) : ControllerBase
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
    ///     Creates user. Only user with lower role can be created
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPost]
    [Authorize]
    public async Task<ActionResult<int>> CreateUser([FromBody] CreateUserRequest request, CreateUserCommandHandler handler, CancellationToken ct)
    {
        var command = mapper.Map<CreateUserCommand>(request);
        return await handler.Handle(command, ct);
    }

    /// <summary>
    ///     Allows to delete any kind of user
    /// </summary>
    /// <description>
    ///     Simple users can be deleted by Admins and above. Admins can be deleted only by superadmins.
    ///     Superadmins can't be deleted from this endpoint
    /// </description>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpDelete("{id:int}")]
    [Authorize]
    public async Task<ActionResult> DeleteUser(int id, DeleteUserHandler handler, CancellationToken ct)
    {
        await handler.Handle(id, ct);
        return NoContent();
    }

    /// <summary>
    ///     Updates user data, if user role is lower
    /// </summary>
    /// <param name="id"></param>
    /// <param name="request"></param>
    [HttpPut("{id:int}")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> UpdateUser(
        [FromRoute] int id,
        [FromBody] UpdateUserRequest request,
        UpdateUserHandler handler,
        CancellationToken ct
    )
    {
        var command = new UpdateUserCommand(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.Password,
            request.Role
        );
        
        var user = await handler.Handle(command, ct);
        var userResponse = mapper.Map<UserResponse>(user);
        return Ok(userResponse);
    }

    /// <summary>
    ///     Updates self profile
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    [HttpPut("profile")]
    [Authorize]
    public async Task<ActionResult<UserResponse>> UpdateProfile(
        [FromBody] UpdateProfileRequest request,
        UpdateProfileHandler handler,
        CancellationToken ct)
    {
        var command = mapper.Map<UpdateProfileCommand>(request);
        var user = await handler.Handle(command, ct);
        var userResponse = mapper.Map<UserResponse>(user);
        return Ok(userResponse);
    }

    /// <summary>
    ///     Updates current user password
    /// </summary>
    /// <param name="request"></param>
    /// <param name="handler"></param>
    /// <param name="ct"></param>
    /// <returns></returns>
    [HttpPut("profile/password")]
    [Authorize]
    public async Task<ActionResult> UpdatePassword(
        [FromBody] UpdatePasswordRequest request,
        UpdatePasswordHandler handler,
        CancellationToken ct
    )
    {
        var command = mapper.Map<UpdatePasswordCommand>(request);
        await handler.Handle(command, ct);
        return Ok();
    }
}