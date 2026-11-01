using System.IO.Ports;

namespace ThabeSoft.EdgeGateway.Channels;


public sealed record class SerialChannelDescriptor : IChannelDescriptor
{
    public ConnectionType ConnectionType => ConnectionType.Serial;

    public required ChannelId Id { get; init; }
    public required string PortName { get; init; }
    public required int BaudRate { get; init; }
    public required int DataBits { get; init; }
    public required Parity Parity { get; init; }
    public required StopBits StopBits { get; init; }


    internal SerialChannelDescriptor() { }
    public static SerialChannelDescriptor Create(ChannelId id, string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNullOrWhiteSpace(portName);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(baudRate);

        if (!Enum.IsDefined(parity))
            throw new ArgumentOutOfRangeException(nameof(parity), $"未知的校验值: {parity}");
        if (dataBits is < 5 or > 8)
            throw new ArgumentOutOfRangeException(nameof(dataBits), $"不支持的数据位: [{dataBits}], 应该在 [5~8] 之中");
        if (stopBits is not (StopBits.One or StopBits.OnePointFive or StopBits.Two))
            throw new ArgumentOutOfRangeException(nameof(stopBits), $"不支持的停止位: {stopBits}");


        return new()
        {
            Id = id,
            PortName = portName,
            BaudRate = baudRate,
            Parity = parity,
            DataBits = dataBits,
            StopBits = stopBits
        };
    }

    public static SerialChannelDescriptor Create(string portName, int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One) =>
        Create(ChannelId.Create(), portName, baudRate, parity, dataBits, stopBits);
}