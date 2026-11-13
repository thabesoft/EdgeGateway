using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortFactory(
        TransportConnectionFactoryResolver transportConnectionFactoryResolver,
        RecoveryPolicyFactoryResolver recoveryPolicyFactoryResolver,
        ProtocolAccessorFactoryResolver protocolAccessorFactoryResolver,
        DeviceFactory deviceFactory,
        ILoggerFactory loggerFactory
    )
{
    public Port Create(PortDescriptor descriptor, ILifecycleStateable gatewayLifecycleState)
    {
        return new
        (
            portDescriptor: descriptor,
            gatewayLifecycleState: gatewayLifecycleState,
            transportConnectionFactoryResolver: transportConnectionFactoryResolver,
            protocolAccessorFactoryResolver: protocolAccessorFactoryResolver,
            recoveryPolicyFactoryResolver: recoveryPolicyFactoryResolver,
            deviceFactory: deviceFactory,
            logger: loggerFactory.CreateLogger<Port>()
        );
    }
}
