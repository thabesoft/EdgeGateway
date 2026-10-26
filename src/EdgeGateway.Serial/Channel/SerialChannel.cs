using EdgeGateway.Byte;
using EdgeGateway.Channel;
using System.IO.Ports;

namespace EdgeGateway.Serial.Channel;


internal sealed class SerialChannel(SerialChannelDescriptor descriptor) : IChannel, IReadBufferDiscardable
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
    public void Discard()
    {
        _serialPort.DiscardInBuffer();
    }


    public ValueTask DisposeAsync()
    {
        _serialPort.Dispose();
        return ValueTask.CompletedTask;
    }
}
