using System.ComponentModel.DataAnnotations;
using Store.Domain.Enums;

namespace Store.API.Contracts.Requests.User;

public record UpdateUserRequest
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

    [Length(1, 50)]
    public string? Password { get; init; }

    public UserRole? Role { get; init; }
}