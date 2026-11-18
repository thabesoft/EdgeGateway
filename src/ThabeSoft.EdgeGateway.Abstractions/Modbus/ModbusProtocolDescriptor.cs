using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Modbus;


public sealed record class ModbusProtocolDescriptor : ProtocolDescriptor
{
    public override ProtocolType Type { get; } = ProtocolType.Modbus;
    public ModbusFrameFormat FrameFormat { get; }
    public TransportType TransportType { get; }


    private ModbusProtocolDescriptor(ModbusFrameFormat frameFormat, TransportType connectionType)
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

    public static ModbusProtocolDescriptor Rtu { get; } = new(ModbusFrameFormat.Rtu, TransportType.Serial);
    public static ModbusProtocolDescriptor Ascii { get; } = new(ModbusFrameFormat.Ascii, TransportType.Serial);
    public static ModbusProtocolDescriptor Ip { get; } = new(ModbusFrameFormat.Ip, TransportType.Ethernet);

    public static ModbusProtocolDescriptor Creaete(ModbusFrameFormat format)
    {
        return format switch
        {
            ModbusFrameFormat.Rtu => Rtu,
            ModbusFrameFormat.Ascii => Ascii,
            ModbusFrameFormat.Ip => Ip,
            _ => throw new InvalidOperationException($"无法创建Modbus协议描述符, 不支持的帧类型({format})")
        };
    }
}