using EdgeGateway.Channel;

namespace EdgeGateway.Device;


public interface IDeviceDescriptor
{
    ProtocolType Protocol { get; }
    ConnectionType Connection { get; }
}