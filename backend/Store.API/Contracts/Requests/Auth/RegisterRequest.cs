using System.ComponentModel.DataAnnotations;
using Store.Domain.Enums;

namespace Store.API.Contracts.Requests.Auth;

/// <summary>
///     Data to let user be registered
/// </summary>
public record RegisterRequest
{
    [Required]
    [Length(1, 50)]
    public required string FirstName { get; init; }
    [Required]
    [Length(1, 50)]
    public required string LastName { get; init; }
    [Required]
    [EmailAddress]
    [Length(1, 50)]
    public required string Email { get; init; }
    [Required]
    [Length(1, 50)]
    public required string Password { get; init; }
}