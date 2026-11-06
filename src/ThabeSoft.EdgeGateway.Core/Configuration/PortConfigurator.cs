using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Configuration;


internal class PortConfigurator(IGateway gateway, IPort port) :
    GatewayConfigurator(gateway), IPortConfigurator
{
    private readonly IGateway _gateway = gateway;

    public IDeviceConfigurator AddDevice(DeviceId deviceId, IProtocolDescriptor protocol)
    {
        var device = port.AddDevice(DeviceDescriptor.Create(deviceId, protocol));
        return new DeviceConfigurator(_gateway, port, device);
    }
}
