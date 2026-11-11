using System.Collections;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Modbus;


public static class ModbusProtocols
{
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
