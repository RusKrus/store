using Store.Domain.Enums;

namespace Application.Commands.Users;

public record UpdateUserCommand(int Id, string FirstName, string LastName, string Email, string Password, UserRole Role);