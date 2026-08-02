using Store.Domain.Enums;

namespace Application.Commands.Auth;

public record RegisterCommand(string FirstName, string LastName, string Email, string Password);