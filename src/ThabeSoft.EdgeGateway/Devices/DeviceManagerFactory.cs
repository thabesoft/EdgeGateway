using Microsoft.Extensions.Logging;
using ThabeSoft.PubSub;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Primitives;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceManagerFactory(
        IEventPublisher eventPublisher,
        ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory,
        ILoggerFactory loggerFactory
    )
{
    public DeviceManager Create(in DeviceManagerCreationArgs args)
    {
        var portId = args.Port.Id;
        var collection = lifecycleKeyedCollectionFactory.Create<DeviceId, IDevice>(args.PortLifecycleState);
        var batchExecutor = lifecycleBatchExecutorFactory.Create(args.DeviceBatchExecutorDescriptor);
        var logger = loggerFactory.CreateLogger<DeviceManager>();

        return new
        (
            portId: portId,
            collection: collection,
            batchExecutor: batchExecutor,
            deviceCreator: args.DevicCreator,
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