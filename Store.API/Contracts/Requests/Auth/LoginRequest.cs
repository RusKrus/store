using System.ComponentModel.DataAnnotations;

namespace Store.API.Contracts.Requests.Auth;

public record LoginRequest
{
    [Required]
    [EmailAddress]
    [Length(1, 50)]
    public required string Email { get; init; }
    [Required]
    [Length(1, 50)]
    public required string Password { get; init; }
}