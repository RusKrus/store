using Store.NotificationsAPI.Features.Notifications.CreateUserEvent;

namespace Store.NotificationsAPI.Features.Notifications;

public static class NotificationHandlers
{
    public static IServiceCollection AddNotificationHandlers(this IServiceCollection services)
    {
        services.AddScoped<CreateUserEventHandler>();

        return services;
    }
}