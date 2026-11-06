using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Configuration;

public interface IPortConfigurator : IGatewayConfigurator
{
    IDeviceConfigurator AddDevice(DeviceId deviceId, IProtocolDescriptor protocol);
}
