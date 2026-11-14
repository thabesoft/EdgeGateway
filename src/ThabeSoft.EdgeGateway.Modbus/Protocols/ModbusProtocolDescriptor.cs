using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Protocols;


public sealed record class ModbusProtocolDescriptor : ProtocolDescriptor
{
    public override ProtocolType Type { get; } = ProtocolType.Modbus;
    public ModbusFrameFormat FrameFormat { get; }
    public ConnectionType ConnectionType { get; }


    public ModbusProtocolDescriptor(ModbusFrameFormat frameFormat, ConnectionType connectionType)
    {
        switch ((frameFormat, connectionType))
        {
            case (ModbusFrameFormat.Rtu, ConnectionType.Serial):
                break;
            case (ModbusFrameFormat.Ascii, ConnectionType.Serial):
                break;
            case (ModbusFrameFormat.Ip, ConnectionType.Ethernet):
                break;
            default: throw new InvalidOperationException($"不支持的Modbus协议组合: ({frameFormat} + {connectionType})");
        }

        FrameFormat = frameFormat;
        ConnectionType = connectionType;
    }
}