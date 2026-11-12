using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionFactory(
        ITriggerFactorySelector triggerFactorySelector,
        ILoggerFactory loggerFactory
    )
{
    public Subscription<T> Create<T>(
            SubscriptionDescriptor<T> subscriptionDescriptor,
            IProtocolAccessor protocolAccessor,
            ILifecycleStateable deviceLifecycleState
        ) where T : notnull
    {
        return new Subscription<T>
        (
            subscriptionDescriptor: subscriptionDescriptor,
            triggerFactorySelector: triggerFactorySelector,
            protocolAccessor: protocolAccessor,
            deviceLifecycleState: deviceLifecycleState,
            logger: loggerFactory.CreateLogger<Subscription<T>>()
        );
    }
}