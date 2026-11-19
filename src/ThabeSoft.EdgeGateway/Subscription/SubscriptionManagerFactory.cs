using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;

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
            logger: logger
        )
        {
            DisposedCallback = async () =>
            {
                await SafeDisposeExtensions.DisposeSafelyAsync(batchExecutor, logger);
                await SafeDisposeExtensions.DisposeSafelyAsync(collection, logger);
            }
        };
    }
}