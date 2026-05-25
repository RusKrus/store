using Store.Domain.Enums;

namespace Store.API.Contracts.Response.Users;

public record UserResponse(int Id, string FullName, string Email, UserRole Role, DateTime CreatedAt);