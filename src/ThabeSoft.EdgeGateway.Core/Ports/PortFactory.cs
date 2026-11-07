using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortFactory(
        TransportConnectionFactorySelector transportConnectionFactorySelector,
        DeviceFactory deviceFactory,
        ILoggerFactory loggerFactory
    )
{
    public Port Create(ITransportDescriptor transportDescriptor)
    {
        return new
        (
            transportDescriptor: transportDescriptor,
            transportConnectionFactorySelector: transportConnectionFactorySelector,
            deviceFactory: deviceFactory,
            logger: loggerFactory.CreateLogger<Port>()
        );
    }
}
