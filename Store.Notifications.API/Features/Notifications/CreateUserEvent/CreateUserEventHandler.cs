using Store.Notifications.Domain.Enums;
using Store.Notifications.Domain.Models;
using Store.Notifications.Infrastructure.Persistance;
using Store.Shared.Bus.EventContracts;

namespace Store.NotificationsAPI.Features.Notifications.CreateUserEvent;

public class CreateUserEventHandler(NotificationsDbContext context)
{
    public async Task HandleAsync(
        UserRegistered @event,
        CancellationToken cancellationToken)
    {
        var notification = Notification.Create(
            @event.EventId,
            @event.Id,
            NotificationTypes.UserRegistered,
            @event.RegisteredAt,
            "New user registration",
            $"New user {@event.UserName} with email {@event.Email} registered in system." );

        await context.Notifications.AddAsync(notification, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }
}