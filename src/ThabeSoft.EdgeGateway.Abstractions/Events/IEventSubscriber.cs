using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.Events;


public interface IEventSubscriber
{
    IDisposable Subscribe<TEvent>(Func<TEvent, CancellationToken, ValueTask> handler) where TEvent : IEvent;
}