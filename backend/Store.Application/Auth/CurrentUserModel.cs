using Store.Domain.Enums;

namespace Application.Auth;

public sealed record CurrentUserModel(int Id, string Email, string FullName, UserRole Role);