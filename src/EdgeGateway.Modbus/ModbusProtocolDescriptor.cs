using EdgeGateway.Channel;
using EdgeGateway.Protocol;

namespace EdgeGateway.Modbus;


public sealed class ModbusProtocolDescriptor : IProtocolDescriptor
{
    public ProtocolType Type { get; } = ProtocolType.Modbus;
    public required ConnectionType Connection { get; init; }
    public required ModbusFrameFormat FrameFormat { get; init; }

    private ModbusProtocolDescriptor() { }



    public static ModbusProtocolDescriptor Rtu { get; } = new()
    {
        Connection = ConnectionType.Serial,
        FrameFormat = ModbusFrameFormat.Rtu
    };
    public static ModbusProtocolDescriptor Ascii { get; } = new()
    {
        Connection = ConnectionType.Serial,
        FrameFormat = ModbusFrameFormat.Ascii
    };
    public static ModbusProtocolDescriptor Ip { get; } = new()
    {
        Connection = ConnectionType.Ethernet,
        FrameFormat = ModbusFrameFormat.Ip
    };
}
