using System.IO.Ports;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Transport;


internal sealed class SerialTransportConnection(SerialTransportDescriptor descriptor) : LifecycleStateMachine, ITransportConnection, IReadBufferDiscardable
{
    private readonly SerialPort _serialPort = new(
        portName: descriptor.PortName,
        baudRate: descriptor.BaudRate,
        parity: descriptor.Parity,
        dataBits: descriptor.DataBits,
        stopBits: descriptor.StopBits);


    public ConnectionEndpoint Endpoint { get; } = descriptor.Endpoint;


    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        _serialPort.BaseStream.ReadAsync(buffer, cancellationToken);

    public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default) =>
        _serialPort.BaseStream.WriteAsync(data, cancellationToken);
    public void DiscardInputBuffer() =>
        _serialPort.DiscardInBuffer();



    protected override ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        _serialPort.Open();
        return ValueTask.CompletedTask;
    }

    protected override ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        _serialPort.Close();
        return ValueTask.CompletedTask;
    }
    protected override ValueTask OnDisposeAsync()
    {
        _serialPort.Dispose();
        return ValueTask.CompletedTask;
    }
}
