using System.IO.Ports;

namespace EdgeGateway.Channels;


public sealed class SerialChannelFactory : IChannelFactory
{
    public bool CanCreate(IChannelDescriptor descriptor)
    {
        return descriptor is SerialChannelDescriptor;
    }

    public IChannel Create(IChannelDescriptor descriptor)
    {
        if (descriptor is not SerialChannelDescriptor serial)
            throw new InvalidOperationException();

        return new SerialChannel(serial);
    }
}

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


internal sealed class SerialChannel(SerialChannelDescriptor descriptor) : IChannel, IInputBufferDiscardable
{
    private readonly SerialPort _serialPort = new(
        portName: descriptor.PortName,
        baudRate: descriptor.BaudRate,
        parity: descriptor.Parity,
        dataBits: descriptor.DataBits,
        stopBits: descriptor.StopBits);

    public ChannelAddress Address { get; } = ChannelAddress.Create(ConnectionType.Serial, descriptor.PortName);


    public ValueTask OpenAsync(CancellationToken cancellationToken = default)
    {
        _serialPort.Open();
        return ValueTask.CompletedTask;
    }

    public ValueTask CloseAsync(CancellationToken cancellationToken = default)
    {
        _serialPort.Close();
        return ValueTask.CompletedTask;
    }

    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _serialPort.BaseStream.ReadAsync(buffer, cancellationToken);
    }

    public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        return _serialPort.BaseStream.WriteAsync(data, cancellationToken);
    }
    public void DiscardInputBuffer()
    {
        _serialPort.DiscardInBuffer();
    }


    public ValueTask DisposeAsync()
    {
        _serialPort.Dispose();
        return ValueTask.CompletedTask;
    }
}
