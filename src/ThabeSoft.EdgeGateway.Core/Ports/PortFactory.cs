using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortFactory(
        TransportConnectionFactorySelector transportConnectionFactorySelector,
        DeviceFactory deviceFactory,
        ProtocolAccessorFactorySelector protocolAccessorFactorySelector,
        ILoggerFactory loggerFactory
    )
{
    public Port Create(ITransportDescriptor transportDescriptor, ILifecycleStateable gatewayLifecycleState)
    {
        return new
        (
            transportDescriptor: transportDescriptor,
            transportConnectionFactorySelector: transportConnectionFactorySelector,
            deviceFactory: deviceFactory,
            protocolAccessorFactorySelector: protocolAccessorFactorySelector,
            gatewayLifecycleState: gatewayLifecycleState,
            logger: loggerFactory.CreateLogger<Port>()
        );
    }
}
