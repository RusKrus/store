using Store.Domain.Enums;

namespace Application.Auth;

public sealed record CurrentUserModel(int id, string Email, string FullName, UserRole Role);