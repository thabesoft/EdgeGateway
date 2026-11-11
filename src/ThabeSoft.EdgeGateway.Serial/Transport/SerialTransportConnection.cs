using Microsoft.Extensions.Logging;
using RJCP.IO.Ports;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Transport;


internal sealed class SerialTransportConnection : ITransportConnection, IReadBufferDiscardable
{
    private readonly IStrictLifecycleStateMachine _stateMachine;
    private readonly SerialPortStream _serialPort;

    public SerialTransportConnection(
        SerialTransportDescriptor descriptor,
        ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        _serialPort = new(
            port: descriptor.PortName,
            baud: descriptor.BaudRate,
            data: descriptor.DataBits,
            parity: descriptor.Parity.ToRjcp(),
            stopbits: descriptor.StopBits.ToRjcp());

        _stateMachine = Lifecycle.StrictStateMachine(logger);
        Endpoint = descriptor.Endpoint;
    }

    public ConnectionEndpoint Endpoint { get; }
    public LifecycleState State => throw new NotImplementedException();


    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync
        (
            method: ct => _serialPort.ReadAsync(buffer, ct),
            cancellationToken: cancellationToken
        );
    }
    public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync
        (
            method: ct => _serialPort.WriteAsync(data, ct),
            cancellationToken: cancellationToken
        );
    }
    public void DiscardInputBuffer()
    {
        _stateMachine.Execute(_serialPort.DiscardInBuffer);
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);



    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        _serialPort.Open();
        return ValueTask.CompletedTask;
    }

    private ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        _serialPort.Close();
        return ValueTask.CompletedTask;
    }
    private async ValueTask OnDisposeAsync()
    {
#if NETSTANDARD2_0
        _serialPort.Dispose();
#else
        await _serialPort.DisposeAsync();
#endif
    }
}