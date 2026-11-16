namespace ThabeSoft.EdgeGateway.Events;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : notnull, IEvent;
}
