using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceManagerFactory(
        IEventPublisher eventPublisher,
        ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory
    )
{
    public DeviceManager Create(in DeviceManagerCreationArgs args)
    {
        return new(
            args: args,
            eventPublisher: eventPublisher,
            lifecycleKeyedCollectionFactory: lifecycleKeyedCollectionFactory,
            lifecycleBatchExecutorFactory: lifecycleBatchExecutorFactory);
    }
}