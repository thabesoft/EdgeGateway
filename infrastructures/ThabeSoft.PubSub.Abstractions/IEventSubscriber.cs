namespace ThabeSoft.PubSub;


public interface IEventSubscriber
{
    IDisposable Subscribe<TEvent>(Func<TEvent, CancellationToken, ValueTask> handler) where TEvent : IMessage;
}