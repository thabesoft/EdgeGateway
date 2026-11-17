using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortManagerFactory(
        IEventPublisher eventPublisher,
        ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory
    )
{
    public PortManager Create(in PortManagerCreationArgs args)
    {
        return new(
            dependencies: args,
            eventPublisher: eventPublisher,
            lifecycleKeyedCollectionFactory: lifecycleKeyedCollectionFactory,
            lifecycleBatchExecutorFactory: lifecycleBatchExecutorFactory);
    }
}