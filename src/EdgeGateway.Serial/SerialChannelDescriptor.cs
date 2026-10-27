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

    /// <inheritdoc/>
    /// <param name="serialPortName">串口名称</param>
    /// <exception cref="ArgumentException"></exception>
    public static SerialChannelDescriptor Create(string serialPortName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(serialPortName);
        return new() { PortName = serialPortName };
    }
}
