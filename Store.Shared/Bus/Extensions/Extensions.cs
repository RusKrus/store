using RabbitMQ.Client;

namespace Store.Shared.Bus.Extensions;

public static class BusExtensions
{
    public static int GetRetryCount(this IReadOnlyBasicProperties basicProperties)
    {
        if (basicProperties.Headers is null ||
            !basicProperties.Headers.TryGetValue("RetryCount", out var retryCount))
        {
            return 0;
        }

        return retryCount switch
        {
            int count => count,
            _ => 0
        };
    }
}