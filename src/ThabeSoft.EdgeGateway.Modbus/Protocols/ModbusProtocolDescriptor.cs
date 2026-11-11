using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Protocols;


public sealed class ModbusProtocolDescriptor : IProtocolDescriptor
{
    public ProtocolType Type { get; } = ProtocolType.Modbus;
    public required ModbusFrameFormat FrameFormat { get; init; }
    public required ConnectionType Connection { get; init; }


    internal ModbusProtocolDescriptor() { }

    // Modbus(Rtu, Serial)
    public override string ToString() => $"Modbus({FrameFormat}, {Connection})";
}