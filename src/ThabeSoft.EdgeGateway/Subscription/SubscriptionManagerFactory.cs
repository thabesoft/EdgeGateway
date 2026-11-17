using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionManagerFactory(
        IEventPublisher eventPublisher,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory,
        ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory
    )
{
    public SubscriptionManager Create(in SubscriptionManagerCreationArgs args)
    {
        return new(
            args: args,
            eventPublisher: eventPublisher,
            lifecycleKeyedCollectionFactory: lifecycleKeyedCollectionFactory,
            lifecycleBatchExecutorFactory: lifecycleBatchExecutorFactory);
    }
}