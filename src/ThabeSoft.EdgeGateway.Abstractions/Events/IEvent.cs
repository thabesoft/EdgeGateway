namespace ThabeSoft.EdgeGateway.Events;

public interface IEvent;

public interface IEventHandler<TEvent> where TEvent : IEvent
{
    ValueTask HandleAsync(TEvent @event, CancellationToken cancellationToken = default);
}