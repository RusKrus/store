using Application.Commands.Auth;
using Application.Common.Errors;
using Application.Common.Queries;
using Store.Application.Interfaces;
using Store.Application.Interfaces.CartCookiesService;
using Store.Domain.Models;
using Store.Infrastructure.RabbitMq.Publishers.EventContracts;

namespace Application.Handlers.Auth;

public class RegisterCommandHandler(
    IUserRepository userRepository,
    ICartRepository cartRepository,
    IUnitOfWork unitOfWork,
    ICartCookiesService cartCookiesService,
    IPasswordHasher passwordHasher,
    IRabbitMqPublisher publisher
    )
{
    public async Task<int> Handle(RegisterCommand command, CancellationToken ct)
    {
        var existingUser = await userRepository.GetByEmailAsync(command.Email, ct);
        if (existingUser != null)
        {
            throw new ConflictException("User with such email already exists");
        }

        var passwordHashed = passwordHasher.Hash(command.Password);
        var user = new User(command.FirstName, command.LastName, command.Email, passwordHashed, null);
        await userRepository.CreateAsync(user, ct);

        var cartGuid = cartCookiesService.GetCartGuidFromCookies();
        if (cartGuid is not null)
        {
            var options = new GetCartByGuidQueryOptions(cartGuid.Value, false, false);
            var newUserCart = await cartRepository.GetByGuidAsync(options, ct);

            if (newUserCart is not null) user.AssignCart(newUserCart);
            cartCookiesService.DeleteCartFromCookies();
        }

        await unitOfWork.SaveChangesAsync(ct);

        await publisher.PublishAsync(
            new UserRegistered(user.Id, user.FullName, user.Email, user.CreatedAt),
            ct);

        return user.Id;
    }
}