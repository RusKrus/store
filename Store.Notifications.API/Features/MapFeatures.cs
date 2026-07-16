using Store.NotificationsAPI.Features.Notifications;

namespace Store.NotificationsAPI.Features;

public static class MapFeatures
{
    public static IServiceCollection MapFeatureFunctions(this IServiceCollection services)
    {
        services
            .AddNotificationHandlers()
            .AddNotificationConsumers();

        return services;
    }

    public static IEndpointRouteBuilder MapFeatureEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapNotificationsEndpoints();

        return app;
    }
}