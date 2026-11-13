using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class DeviceFactory(
    SubscriptionFactory subscriptionFactory,
    ILoggerFactory loggerFactory)
{
    public Device Create(DeviceDescriptor deviceDescriptor, ILifecycleStateable portLifecycleState, IProtocolAccessor protocolAccessor, RecoveryPolicyHandler recoveryPolicyHandler)
    {
        return new
        (
            deviceDescriptor: deviceDescriptor,
            subscriptionFactory: subscriptionFactory,
            portLifecycleState: portLifecycleState,
            protocolAccessor: protocolAccessor,
            recoveryPolicyHandler: recoveryPolicyHandler,
            stateMachine: Lifecycle.RetryableStateMachine(),
            logger: loggerFactory.CreateLogger<Device>()
        );
    }
}