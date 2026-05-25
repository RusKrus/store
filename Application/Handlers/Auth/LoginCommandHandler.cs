using Application.Commands.Auth;
using Application.Common.Errors;
using Store.Application.Interfaces;

namespace Application.Handlers.Auth;

public class LoginCommandHandler(IUserRepository repository, IPasswordHasher passwordHasher, IJwtProvider jwtProvider)
{
    public async Task<string> Handle(LoginCommand command, CancellationToken ct)
    {
        var user = await repository.GetByEmailAsync(command.Email, ct);
        if (user is null)
        {
            throw new NotFoundException($"User with email {command.Email} not found");
        }
        var isPasswordValid = passwordHasher.Verify(command.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new ValidationException("Invalid password");
        }

        return jwtProvider.Generate(user);
    }
}