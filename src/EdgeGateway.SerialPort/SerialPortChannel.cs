using EdgeGateway.Bytes;
using EdgeGateway.Channel;

namespace EdgeGateway.SerialPort;

public sealed class SerialPortChannel : IChannel
{
    private readonly System.IO.Ports.SerialPort _serialPort;
    private readonly StreamByteReader _reader;
    private readonly StreamByteWriter _writer;


    public bool IsOpen => _serialPort.IsOpen;


    public SerialPortChannel(SerialPortDescriptor descriptor)
    {
        _serialPort = new(descriptor.PortName, descriptor.BaudRate, descriptor.Parity, descriptor.DataBits, descriptor.StopBits);

        _reader = StreamByteReader.Create(_serialPort.BaseStream);
        _writer = StreamByteWriter.Create(_serialPort.BaseStream);
    }


    public ValueTask OpenAsync(CancellationToken ct = default)
    {
        if (IsOpen) return ValueTask.CompletedTask;

        _serialPort.Open();
        return ValueTask.CompletedTask;
    }
    public ValueTask CloseAsync(CancellationToken ct = default)
    {
        if (!IsOpen) return ValueTask.CompletedTask;

        _serialPort.Close();
        return ValueTask.CompletedTask;
    }


    public ValueTask ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _reader.ReadExactlyAsync(buffer, cancellationToken);
    }

    public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        return _writer.WriteAsync(data, cancellationToken);
    }


    public async ValueTask DisposeAsync()
    {
        await CloseAsync();
        _serialPort.Dispose();
    }
}