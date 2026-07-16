using Store.NotificationsAPI.Features.Notifications.CreateUserEvent;

namespace Store.NotificationsAPI.Features.Notifications;

public static class NotificationConsumers
{
    public static IServiceCollection AddNotificationConsumers(this IServiceCollection services)
    {
        services.AddHostedService<CreateUserEventConsumer>();
        return services;
    }
}