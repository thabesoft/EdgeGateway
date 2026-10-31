using System.IO.Ports;

namespace EdgeGateway.Options;

public sealed class SerialPortSlaveHostedServiceOptions
{
    public TimeSpan TickInterval { get; set; } = TimeSpan.FromMilliseconds(100);
    public required string PortName { get; set; }
    public int BaudRate { get; set; } = 9600;
    public Parity Parity { get; set; } = Parity.None;
    public int DataBits { get; set; } = 8;
    public StopBits StopBits { get; set; } = StopBits.One;
}