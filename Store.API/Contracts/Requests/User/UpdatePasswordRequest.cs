using System.ComponentModel.DataAnnotations;

namespace Store.API.Contracts.Requests.User;

public record UpdatePasswordRequest
{
    [Required]
    [Length(1, 50)]
    public required string OldPassword { get; init; }
    [Required]
    [Length(1, 50)]
    public required string NewPassword { get; init; }
};