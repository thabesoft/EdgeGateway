using System.IO.Ports;

namespace ThabeSoft.EdgeGateway.Channels;


public sealed record class SerialChannelDescriptor : IChannelDescriptor
{
    public required ChannelAddress Address { get; init; }
    public required string PortName { get; init; }
    public required int BaudRate { get; init; }
    public required int DataBits { get; init; }
    public required Parity Parity { get; init; } 
    public required StopBits StopBits { get; init; }


    internal SerialChannelDescriptor() { }
    public static SerialChannelDescriptorBuilder Builder() => new();
}


public sealed class SerialChannelDescriptorBuilder
{
    private string? _portName;
    private int? _baudRate;
    private int? _dataBits;
    private Parity? _parity;
    private StopBits? _stopBits;


    public SerialChannelDescriptorBuilder PortName(string portName)
    {
        if (string.IsNullOrWhiteSpace(portName))
            throw new ArgumentException("端口名称不可为空", nameof(portName));

        _portName = portName;
        return this;
    }

    public SerialChannelDescriptorBuilder DataBits(int dataBits)
    {
        if (dataBits is < 5 or > 8)
            throw new ArgumentOutOfRangeException(nameof(dataBits), "数据位必须在 5~8 之间。");

        _dataBits = dataBits;
        return this;
    }

    public SerialChannelDescriptorBuilder BaudRate(int baudRate)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(baudRate);

        _baudRate = baudRate;
        return this;
    }

    public SerialChannelDescriptorBuilder Parity(Parity parity)
    {
        if (!Enum.IsDefined(parity))
            throw new ArgumentOutOfRangeException(nameof(parity));

        _parity = parity;
        return this;
    }

    public SerialChannelDescriptorBuilder StopBits(StopBits stopBits)
    {
        if (!Enum.IsDefined(stopBits))
            throw new ArgumentOutOfRangeException(nameof(stopBits));

        if (stopBits == System.IO.Ports.StopBits.None)
            throw new ArgumentException("停止位不可为 None", nameof(stopBits));

        _stopBits = stopBits;
        return this;
    }



    public SerialChannelDescriptor Build()
    {
        if (string.IsNullOrWhiteSpace(_portName))
            throw new ArgumentException("未设置端口名称", nameof(_portName));

        if (_baudRate is not int baudRate)
            throw new ArgumentNullException(nameof(_baudRate), "未设置波特率");

        if (_dataBits is not int dataBits)
            throw new ArgumentNullException(nameof(_baudRate), "未设置数据位");

        if (_parity is not Parity parity)
            throw new ArgumentNullException(nameof(_baudRate), "未设置校验模式");

        if (_stopBits is not StopBits stopBits)
            throw new ArgumentNullException(nameof(_baudRate), "未设置停止位");


        return new SerialChannelDescriptor()
        {
            PortName = _portName!,
            BaudRate = baudRate,
            DataBits = dataBits,
            Parity = parity,
            StopBits = stopBits,
            Address = ChannelAddress.Create(ConnectionType.Serial, _portName!)
        };
    }
}