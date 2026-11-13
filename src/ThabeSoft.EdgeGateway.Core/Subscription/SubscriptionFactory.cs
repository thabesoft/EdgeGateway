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
            RecoveryPolicyHandler recoveryPolicyHandler
        ) where T : notnull
    {
        return new Subscription<T>
        (
            subscriptionDescriptor: subscriptionDescriptor,
            deviceLifecycleState: deviceLifecycleState,
            recoveryPolicyHandler: recoveryPolicyHandler,
            protocolAccessor: protocolAccessor,
            triggerScheduler: triggerScheduler,
            logger: loggerFactory.CreateLogger<Subscription<T>>()
        );
    }
}