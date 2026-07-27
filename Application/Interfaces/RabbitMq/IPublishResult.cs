using System.Runtime.InteropServices.JavaScript;

namespace Store.Application.Interfaces.RabbitMq;

public interface IPublishResult<T>
{
    T Message { get; }
    bool IsSuccessful { get; }
    Exception? Exception { get; }
}