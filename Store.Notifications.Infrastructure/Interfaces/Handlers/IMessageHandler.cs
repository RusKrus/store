namespace Store.Notifications.Infrastructure.Interfaces.Handlers;

public interface IMessageHandler<in TMessage>
{
    Task HandleAsync(TMessage message, CancellationToken cancellationToken);
}