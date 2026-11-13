using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class DeviceSubscriptionFactory(
        ITriggerScheduler triggerScheduler,
        ILoggerFactory loggerFactory
    )
{
    public Subscription<T> Create<T>(
            SubscriptionDescriptor<T> subscriptionDescriptor,
            SubscriptionReader<T> subscriptionReader
        ) where T : notnull
    {
        return new Subscription<T>
        (
            subscriptionDescriptor: subscriptionDescriptor,
            subscriptionReader: subscriptionReader,
            triggerScheduler: triggerScheduler,
            stateMachine: Lifecycle.StrictStateMachine(),
            logger: loggerFactory.CreateLogger<Subscription<T>>()
        );
    }
}