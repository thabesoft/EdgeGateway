using EdgeGateway.Bytes;

namespace EdgeGateway.Devices;


/// <summary>
/// 通讯设备配置，包含设备的通信协议和端口配置
/// </summary>
public sealed record class DeviceDescriptor
{
    public required ChannelDescriptor PortDescriptor { get; init; }

    private DeviceDescriptor() { }


    public static DeviceDescriptor Create(ChannelDescriptor portDescriptor)
    {
        return new DeviceDescriptor
        {
            PortDescriptor = portDescriptor
        };
    }
}