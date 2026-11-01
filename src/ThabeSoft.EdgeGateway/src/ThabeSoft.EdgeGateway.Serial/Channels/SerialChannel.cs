using System.IO.Ports;
using ThabeSoft.EdgeGateway.IO;

namespace ThabeSoft.EdgeGateway.Channels;


internal sealed class SerialChannel(SerialChannelDescriptor descriptor) : IChannel, IReadBufferDiscardable
{
    private readonly SerialPort _serialPort = new(
        portName: descriptor.PortName,
        baudRate: descriptor.BaudRate,
        parity: descriptor.Parity,
        dataBits: descriptor.DataBits,
        stopBits: descriptor.StopBits);


    public ChannelId Id { get; } = descriptor.Id;
    public ChannelAddress Address { get; } = ChannelAddress.Create(descriptor.ConnectionType, descriptor.PortName);
    public bool IsOpen => _serialPort.IsOpen;


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
