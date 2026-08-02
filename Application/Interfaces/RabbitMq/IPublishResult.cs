using System.Runtime.InteropServices.JavaScript;
using Store.Shared.Bus.EventContracts;

namespace Store.Application.Interfaces.RabbitMq;

public interface IPublishResult
{
    IOutboxPublishMessage Message { get; }
    bool IsSuccessful { get; }
    Exception? Exception { get; }
}