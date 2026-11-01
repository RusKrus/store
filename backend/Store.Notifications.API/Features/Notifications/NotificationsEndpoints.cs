namespace Store.NotificationsAPI.Features.Notifications;

public static class NotificationsEndpoints
{
    public static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/notifications")
            .WithTags("Notifications");

        return app;
    }
}