using EdgeGateway.Ports;
using EdgeGateway.Protocols;

namespace EdgeGateway.Devices;


/// <summary>
/// 设备配置，包含设备的通信协议和端口配置
/// </summary>
public sealed record class DeviceConfig(DeviceId DeviceId, ProtocolType Protocol, PortAddress PortAddress)
{
    public static DeviceConfig Create(ProtocolType protocolType, PortAddress address)
    {
        return new DeviceConfig(DeviceId.New(), protocolType, address);
    }
}