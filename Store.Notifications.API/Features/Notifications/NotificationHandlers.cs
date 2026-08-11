using Store.Notifications.Infrastructure.Interfaces.Handlers;
using Store.NotificationsAPI.Features.Notifications.CreateUserEvent;
using Store.Shared.Bus.EventContracts;

namespace Store.NotificationsAPI.Features.Notifications;

public static class NotificationHandlers
{
    public static IServiceCollection AddNotificationHandlers(this IServiceCollection services)
    {
        services.AddScoped<IMessageHandler<UserRegistered>, CreateUserEventHandler>();

        return services;
    }
}