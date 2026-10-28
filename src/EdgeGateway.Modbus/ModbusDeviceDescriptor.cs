using EdgeGateway.Channel;
using EdgeGateway.Device;

namespace EdgeGateway.Modbus;


public sealed record class ModbusDeviceDescriptor : IDeviceDescriptor
{
    public ProtocolType Protocol { get; } = ProtocolType.Modbus;
    public required ConnectionType Connection { get; init; }
    public required ModbusFrameFormat FrameFormat { get; init; }

    private ModbusDeviceDescriptor() { }



    public static ModbusDeviceDescriptor Rtu { get; } = new()
    {
        Connection = ConnectionType.Serial,
        FrameFormat = ModbusFrameFormat.Rtu
    };
    public static ModbusDeviceDescriptor Ascii { get; } = new()
    {
        Connection = ConnectionType.Serial,
        FrameFormat = ModbusFrameFormat.Ascii
    };
    public static ModbusDeviceDescriptor Ip { get; } = new()
    {
        Connection = ConnectionType.Ip,
        FrameFormat = ModbusFrameFormat.Ip
    };
}
