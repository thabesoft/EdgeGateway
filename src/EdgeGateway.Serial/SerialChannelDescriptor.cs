using EdgeGateway.Channel;
using System.IO.Ports;

namespace EdgeGateway.Serial;


public sealed record class SerialChannelDescriptor : IChannelDescriptor
{
    public required string PortName { get; init; }
    public int BaudRate { get; init; } = 9600;
    public int DataBits { get; init; } = 8;
    public Parity Parity { get; init; } = Parity.None;
    public StopBits StopBits { get; init; } = StopBits.One;


    private SerialChannelDescriptor() { }

    public static SerialChannelDescriptor Create(string portName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(portName);
        return new() { PortName = portName };
    }
}
