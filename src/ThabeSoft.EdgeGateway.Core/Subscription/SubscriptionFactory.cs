using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionFactory(
        ITriggerScheduler triggerScheduler,
        ILoggerFactory loggerFactory
    )
{
    public Subscription<T> Create<T>(
            SubscriptionDescriptor<T> subscriptionDescriptor,
            IProtocolAccessor protocolAccessor,
            ILifecycleStateable deviceLifecycleState,
            RecoveryPolicyHandler exceptionHandler
        ) where T : notnull
    {
        return new Subscription<T>
        (
            subscriptionDescriptor: subscriptionDescriptor,
            triggerScheduler: triggerScheduler,
            protocolAccessor: protocolAccessor,
            deviceLifecycleState: deviceLifecycleState,
            recoveryPolicyHandler: exceptionHandler,
            logger: loggerFactory.CreateLogger<Subscription<T>>()
        );
    }
}