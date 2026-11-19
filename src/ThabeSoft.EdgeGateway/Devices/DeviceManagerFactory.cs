using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;

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