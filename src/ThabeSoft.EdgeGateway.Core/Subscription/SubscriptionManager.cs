using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport.Byte;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionManager(
        TriggerManager triggerManager,
        ProtocolAccessorManager protocolAccessorManager,
        ILoggerFactory loggerFactory
    ) : IAsyncDisposable
{
    private readonly ILogger<SubscriptionManager> _logger = loggerFactory.CreateLogger<SubscriptionManager>();
    private readonly ConcurrentDictionary<SubscriptionId, ISubscription> _items = [];


    public ISubscription<T> Register<T>(
            SubscriptionDescriptor<T> subscriptionDescriptor,
            IProtocolDescriptor protocolDescriptor,
            IByteDuplex byteDuplex
        ) where T : notnull
    {
        var trigger = triggerManager.Register(subscriptionDescriptor.Trigger);
        var protocolAccessor = protocolAccessorManager.Register(protocolDescriptor, byteDuplex);
        var logger = loggerFactory.CreateLogger<Subscription<T>>();

        var subscription = new Subscription<T>
        (
            descriptor: subscriptionDescriptor,
            trigger: trigger,
            getValueHandler: ct => protocolAccessor.ReadAsync(subscriptionDescriptor.Tag, ct),
            logger: logger
        );


        var entry = new Entry<T>(subscription, trigger, this, _logger);
        if (_items.TryAdd(entry.Id, entry)) return entry;

        entry.Dispose();
        throw new InvalidOperationException();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var i in _items.Values)
            await i.DisposeAsync();

        if (_logger.IsEnabled(LogLevel.Debug))
            _logger.LogDebug("订阅管理器已释放");
    }

    public sealed class Entry<T>(
            ISubscription<T> subscription,
            ITrigger trigger,
            SubscriptionManager manager,
            ILogger logger
        ) : ISubscription<T>, IDisposable
        where T : notnull
    {
        private int _disposed;

        public async ValueTask DisposeAsync()
        {
            if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

            await subscription.DisposeAsync();
            await trigger.DisposeAsync();
            //await protocolAccessor.DisposeAsync();

            manager._items.TryRemove(Id, out _);

            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("订阅 [{Id}] 已释放", Id);
        }
        public async void Dispose()
        {
            await DisposeAsync();
        }



        public SubscriptionId Id => subscription.Id;
        public IDataAddress Address => subscription.Address;
        public LifecycleState State => subscription.State;
        public Reading<T>? Latest => subscription.Latest;
        Reading? ISubscription.Latest => Latest;


        public IDisposable Subscribe(IObserver<Reading<T>> observer)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("订阅 [{Id}] 添加订阅者", Id);

            return subscription.Subscribe(observer);
        }

        public ValueTask StartAsync(CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("订阅 [{Id}] 启动", Id);

            return subscription.StartAsync(cancellationToken);
        }
        public ValueTask StopAsync(CancellationToken cancellationToken = default)
        {
            if (logger.IsEnabled(LogLevel.Debug))
                logger.LogDebug("订阅 [{Id}] 停止", Id);

            return subscription.StopAsync(cancellationToken);
        }
    }
}
