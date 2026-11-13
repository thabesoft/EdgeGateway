using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortFactory(
        TransportConnectionFactoryResolver transportConnectionFactoryResolver,
        DeviceFactory deviceFactory,
        ProtocolAccessorFactoryResolver protocolAccessorFactoryResolver,
        ILoggerFactory loggerFactory
    )
{
    public Port Create(ITransportDescriptor transportDescriptor, ILifecycleStateable gatewayLifecycleState)
    {
        return new
        (
            gatewayLifecycleState: gatewayLifecycleState,
            transportDescriptor: transportDescriptor,
            transportConnectionFactoryResolver: transportConnectionFactoryResolver,
            protocolAccessorFactoryResolver: protocolAccessorFactoryResolver,
            deviceFactory: deviceFactory,
            logger: loggerFactory.CreateLogger<Port>()
        );
    }
}
