using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Protocols;


public sealed class ModbusProtocolDescriptor : IProtocolDescriptor
{
    public ProtocolType Type { get; } = ProtocolType.Modbus;
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
    public bool Equals(IProtocolDescriptor other)
    {
        if (other is not ModbusProtocolDescriptor modbus) return false;
        return modbus.FrameFormat == FrameFormat && modbus.ConnectionType == ConnectionType;
    }

    // Modbus(Rtu, Serial)
    public override string ToString() => $"Modbus({FrameFormat}, {ConnectionType})";
}