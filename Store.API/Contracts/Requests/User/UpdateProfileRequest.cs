using System.ComponentModel.DataAnnotations;
using Store.Domain.Enums;

namespace Store.API.Contracts.Requests.User;

public record UpdateProfileRequest
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
}