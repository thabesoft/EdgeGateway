using EdgeGateway.Devices;
using EdgeGateway.Ports;
using EdgeGateway.Protocols;

namespace EdgeGateway.Options;


/// <summary>
/// 设备配置，包含设备的通信协议和端口配置
/// </summary>
public sealed record class DeviceOptions(DeviceId DeviceId, ProtocolType Protocol, PortAddress PortAddress)
{
    public static DeviceOptions Create(ProtocolType protocolType, PortAddress address)
    {
        return new DeviceOptions(DeviceId.New(), protocolType, address);
    }
}