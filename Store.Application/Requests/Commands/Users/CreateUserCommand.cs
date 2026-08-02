using Store.Domain.Enums;

namespace Application.Commands.Users;

public record CreateUserCommand(string FirstName, string LastName, string Email, string Password, UserRole Role);