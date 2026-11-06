using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Protocols;


public sealed class ModbusProtocolDescriptor : IProtocolDescriptor
{
    public ProtocolType Type { get; } = ProtocolType.Modbus;
    public required ConnectionType Connection { get; init; }
    public required ModbusFrameFormat FrameFormat { get; init; }

    internal ModbusProtocolDescriptor() { }
}
