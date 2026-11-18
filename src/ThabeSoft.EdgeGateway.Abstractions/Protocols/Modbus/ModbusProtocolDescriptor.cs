using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Protocols.Modbus;


public sealed record class ModbusProtocolDescriptor : ProtocolDescriptor
{
    public override ProtocolType Type { get; } = ProtocolType.Modbus;
    public ModbusFrameFormat FrameFormat { get; }
    public TransportType TransportType { get; }


    public ModbusProtocolDescriptor(ModbusFrameFormat frameFormat, TransportType connectionType)
    {
        switch ((frameFormat, connectionType))
        {
            case (ModbusFrameFormat.Rtu, TransportType.Serial):
                break;
            case (ModbusFrameFormat.Ascii, TransportType.Serial):
                break;
            case (ModbusFrameFormat.Ip, TransportType.Ethernet):
                break;
            default: throw new InvalidOperationException($"不支持的Modbus协议组合: ({frameFormat} + {connectionType})");
        }

        FrameFormat = frameFormat;
        TransportType = connectionType;
    }
}