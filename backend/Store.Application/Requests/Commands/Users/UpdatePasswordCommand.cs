namespace Application.Commands.Users;

public record UpdatePasswordCommand(string OldPassword, string NewPassword);