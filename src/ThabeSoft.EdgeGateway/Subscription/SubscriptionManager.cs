using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Messages;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.PubSub;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionManager(SubscriptionManagerDependencies dependencies) : IAsyncDisposable
{
    private readonly DisposeFlag _disposeFlag = new();


    public IReadOnlyDictionary<SubscriptionId, ISubscription> Items => dependencies.Collection.Items;


    public ValueTask StartAllAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return dependencies.BatchExecutor.StartBatchAsync(dependencies.Collection.Values, cancellationToken);
    }
    public ValueTask StopAllAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return dependencies.BatchExecutor.StopBatchAsync(dependencies.Collection.Values, cancellationToken);
    }

    public async ValueTask<ISubscription<T>> AddAsync<T>(SubscriptionDescriptor<T> descriptor, CancellationToken cancellationToken = default)
        where T : notnull
    {
        _disposeFlag.ThrowIfDisposed(this);

        var subscription = await dependencies.Collection.AddAsync(descriptor.Id, () => 
            dependencies.SubscriptionCreator.Create(descriptor), cancellationToken);

        _ = dependencies.EventPublisher.PublishAsync(new SubscriptionAddedMessage(dependencies.Port.Id, dependencies.Device.Id, subscription.Id), cancellationToken);

        return (ISubscription<T>)subscription;
    }
    public async ValueTask<bool> RemoveAsync(SubscriptionId id, CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);

        if (await dependencies.Collection.RemoveAsync(id, cancellationToken))
        {
            _ = dependencies.EventPublisher.PublishAsync(new SubscriptionRemovedMessage(dependencies.Port.Id, dependencies.Device.Id, id), cancellationToken);
            return true;
        }
        return false;
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposeFlag.TryMarkDisposed()) return;
        await Safe.DisposeSafelyAsync(dependencies.OnDispose, dependencies.Logger);
    }
}


internal sealed class SubscriptionManagerDependencies
{
    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }

    // Device
    public required DeviceDescriptor Device { get; init => field = value.Required(); }

    // Subscription
    public required ILifecycleKeyedCollection<SubscriptionId, ISubscription> Collection { get; init => field = value.Required(); }
    public required ILifecycleBatchExecutor BatchExecutor { get; init => field = value.Required(); }
    public required ISubscriptionCreator SubscriptionCreator { get; init => field = value.Required(); }


    // Services
    public required IEventPublisher EventPublisher { get; init => field = value.Required(); }
    public required ILogger<SubscriptionManager> Logger { get; init => field = value.Required(); }
    public required Func<ValueTask> OnDispose { get; init => field = value.Required(); }
}

internal interface ISubscriptionCreator
{
    ISubscription<T> Create<T>(SubscriptionDescriptor<T> descriptor) where T : notnull;
}