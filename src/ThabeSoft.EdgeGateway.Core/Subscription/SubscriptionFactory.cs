using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport.Byte;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionFactory(
        ITriggerFactorySelector triggerFactorySelector,
        SubscriptionValueReaderFactory subscriptionValueReaderFactory,
        ILoggerFactory loggerFactory
    )
{
    public ISubscription<T> Create<T>(
            SubscriptionDescriptor<T> subscriptionDescriptor,
            IProtocolDescriptor protocolDescriptor,
            IByteDuplex byteDuplex,
            ILifecycleStateable deviceLifecycleState
        ) where T : notnull
    {
        return new Subscription<T>
        (
            subscriptionDescriptor: subscriptionDescriptor,
            protocolDescriptor: protocolDescriptor,
            triggerFactorySelector: triggerFactorySelector,
            byteDuplex: byteDuplex,
            deeviceLifecycleState: deviceLifecycleState,
            subscriptionValueReaderFactory: subscriptionValueReaderFactory,
            logger: loggerFactory.CreateLogger<Subscription<T>>()
        );
    }
}