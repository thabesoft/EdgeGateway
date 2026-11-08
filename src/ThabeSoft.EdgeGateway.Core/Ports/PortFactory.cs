using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortFactory(
        TransportConnectionFactorySelector transportConnectionFactorySelector,
        DeviceFactory deviceFactory,
        ILoggerFactory loggerFactory
    )
{
    public Port Create(ITransportDescriptor transportDescriptor, ILifecycleState gatewayLifecycleState)
    {
        return new
        (
            transportDescriptor: transportDescriptor,
            transportConnectionFactorySelector: transportConnectionFactorySelector,
            gatewayLifecycleState: gatewayLifecycleState,
            deviceFactory: deviceFactory,
            logger: loggerFactory.CreateLogger<Port>()
        );
    }
}
