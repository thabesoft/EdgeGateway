using ThabeSoft.EdgeGateway.Triggers;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Polling;


internal sealed class SubscriptionManager(TriggerManager triggerManager, ProtocolAccessorManager protocolAccessorManager, ILoggerFactory loggerFactory)
{
    private readonly ConcurrentDictionary<ISubscription, byte> _values = [];
    public IReadOnlyCollection<ISubscription> Subscriptions => [.. _values.Keys];


    public async ValueTask<ISubscription<T>> RegisterAsync<T>(SubscriptionDescriptor<T> subscriptionDescriptor, IProtocolDescriptor protocolDescriptor, IByteDuplex byteDuplex) where T : notnull
    {
        var trigger = await triggerManager.RegisterAsync(subscriptionDescriptor.Trigger);
        var tag = new Tag<T>(subscriptionDescriptor.Tag);
        var protocolAccessor = protocolAccessorManager.Register(protocolDescriptor, byteDuplex);

        var subscription = new Subscription<T>(trigger, tag, async ct =>
        {
            var value = await protocolAccessor.ReadAsync(subscriptionDescriptor.Tag, ct);
            tag.Update(value);

        }, loggerFactory.CreateLogger<Subscription<T>>());


        var entry = new Entry<T>(subscription, trigger, tag, this);
        if (_values.TryAdd(entry, 0)) return entry;

        await trigger.DisposeAsync();
        throw new InvalidOperationException();
    }

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public sealed class Entry<T>(ISubscription subscription, ITrigger trigger, ITag<T> tag, SubscriptionManager manager) : ISubscription<T>
        where T : notnull
    {
        public async ValueTask DisposeAsync()
        {
            await subscription.DisposeAsync();
            await trigger.DisposeAsync();

            manager._values.TryRemove(subscription, out _);
        }


        public LifecycleState State => subscription.State;
        public ITag<T> Tag => tag;
        ITag ISubscription.Tag => Tag;


        public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
            subscription.StartAsync(cancellationToken);
        public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
            subscription.StopAsync(cancellationToken);
    }
}
