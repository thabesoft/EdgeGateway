using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Primitives;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionManagerFactory(
        IEventPublisher eventPublisher,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory,
        ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
        ILoggerFactory loggerFactory
    )
{
    public SubscriptionManager Create(in SubscriptionManagerCreationArgs args)
    {
        var portId = args.Port.Id;
        var deviceId = args.Device.Id;
        var collection = lifecycleKeyedCollectionFactory.Create<SubscriptionId, ISubscription>(args.DeviceState);
        var batchExecutor = lifecycleBatchExecutorFactory.Create(args.SubscriptionBatchExecutor);
        var logger = loggerFactory.CreateLogger<SubscriptionManager>();

        return new
        (
            portId: portId,
            deviceId: deviceId,
            collection: collection,
            batchExecutor: batchExecutor,
            subscriptionCreator: args.SubscriptionCreator,
            eventPublisher: eventPublisher,
            logger: logger,
            onDispose: async () =>
            {
                await Safe.DisposeSafelyAsync(batchExecutor, logger);
                await Safe.DisposeSafelyAsync(collection, logger);
            }
        );
    }
}

internal readonly struct SubscriptionManagerCreationArgs
{
    public required PortDescriptor Port { get; init => field = value.Required(); }

    public required DeviceDescriptor Device { get; init => field = value.Required(); }
    public required ILifecycleStateable DeviceState { get; init => field = value.Required(); }

    public required ISubscriptionCreator SubscriptionCreator { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor SubscriptionBatchExecutor { get; init => field = value.Required(); }
}

internal interface ISubscriptionCreator
{
    Subscription<T> Create<T>(SubscriptionDescriptor<T> descriptor) where T : notnull;
}