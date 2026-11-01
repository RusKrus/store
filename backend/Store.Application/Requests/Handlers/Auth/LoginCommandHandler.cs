using Application.Commands.Auth;
using Application.Common.Errors;
using Application.Common.Queries;
using Store.Application.Interfaces;
using Store.Application.Interfaces.CartCookiesService;
using Store.Domain.Enums;
using Store.Domain.Models;

namespace Application.Handlers.Auth;

public class LoginCommandHandler(
    IUserRepository userRepository,
    ICartRepository cartRepository,
    IPasswordHasher passwordHasher,
    ICartCookiesService cartCookiesService,
    IJwtProvider jwtProvider,
    IUnitOfWork unitOfWork)
{
    public async Task<string> Handle(LoginCommand command, CancellationToken ct)
    {
        var user = await userRepository.GetByEmailAsync(command.Email, ct);
        if (user is null)
        {
            throw new NotFoundException($"User with email {command.Email} not found");
        }
        var isPasswordValid = passwordHasher.Verify(command.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new ValidationException("Invalid password");
        }

        var cartGuid = cartCookiesService.GetCartGuidFromCookies();
        if (cartGuid is not null)
        {
            var options = new GetCartByGuidQueryOptions(cartGuid.Value, true, true);
            var cartFromCookies = await cartRepository.GetByGuidAsync(options, ct);
            if (cartFromCookies is not null)
            {
                var result = user.AssignCart(cartFromCookies);
                if (result == CartsMergeStatuses.CartsMerged) await cartRepository.DeleteByIdAsync(cartFromCookies.Id, ct);
            }
        }

        cartCookiesService.DeleteCartFromCookies();
        await unitOfWork.SaveChangesAsync(ct);
        return jwtProvider.Generate(user);
    }
}