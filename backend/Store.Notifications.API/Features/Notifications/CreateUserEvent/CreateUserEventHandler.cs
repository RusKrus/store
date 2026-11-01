using Store.Notifications.Domain.Enums;
using Store.Notifications.Domain.Models;
using Store.Notifications.Infrastructure.Interfaces.Handlers;
using Store.Notifications.Infrastructure.Persistance;
using Store.Shared.Bus.EventContracts;

namespace Store.NotificationsAPI.Features.Notifications.CreateUserEvent;

public class CreateUserEventHandler(NotificationsDbContext context) : IMessageHandler<UserRegistered>
{
    public async Task HandleAsync(
        UserRegistered message,
        CancellationToken cancellationToken)
    {
        var notification = Notification.Create(
            message.EventId,
            message.Id,
            NotificationTypes.UserRegistered,
            message.RegisteredAt,
            "New user registration",
            $"New user {message.UserName} with email {message.Email} registered in system." );

        await context.Notifications.AddAsync(notification, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}