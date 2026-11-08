using Microsoft.Extensions.Logging;
using RJCP.IO.Ports;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport.Byte;

namespace ThabeSoft.EdgeGateway.Transport;


internal sealed class SerialTransportConnection(
        SerialTransportDescriptor descriptor,
        ILogger logger
    ) : ITransportConnection, IReadBufferDiscardable
{
    private readonly LifecycleStateMachineA _stateMachine = Lifecycle.Normal(x => x.Start(30).Stop(15), logger: logger);
    private readonly SerialPortStream _serialPort = new(
        port: descriptor.PortName,
        baud: descriptor.BaudRate,
        data: descriptor.DataBits,
        parity: descriptor.Parity.ToRjcp(),
        stopbits: descriptor.StopBits.ToRjcp());

    public ConnectionEndpoint Endpoint { get; } = descriptor.Endpoint;
    public LifecycleState State => throw new NotImplementedException();


    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync
        (
            state: (SerialPort: _serialPort, Buffer: buffer),
            action: static (state, ct) => state.SerialPort.ReadAsync(state.Buffer, ct),
            cancellationToken: cancellationToken
        );
    }
    public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        return _stateMachine.ExecuteAsync
        (
            state: (SerialPort: _serialPort, Data: data),
            action: static (state, ct) => state.SerialPort.WriteAsync(state.Data, ct),
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


    private static RJCP.IO.Ports.Parity ToRjcp(SerialParity parity) => parity switch
    {
        SerialParity.None => RJCP.IO.Ports.Parity.None,
        SerialParity.Even => RJCP.IO.Ports.Parity.Even,
        SerialParity.Odd => RJCP.IO.Ports.Parity.Odd,
        SerialParity.Mark => RJCP.IO.Ports.Parity.Mark,
        SerialParity.Space => RJCP.IO.Ports.Parity.Space,
        _ => throw new ArgumentOutOfRangeException(nameof(parity))
    };
    public static RJCP.IO.Ports.StopBits ToRjcp(SerialStopBits stopBits) => stopBits switch
    {
        SerialStopBits.One => RJCP.IO.Ports.StopBits.One,
        SerialStopBits.Two => RJCP.IO.Ports.StopBits.Two,
        SerialStopBits.OnePointFive => RJCP.IO.Ports.StopBits.One5,
        _ => throw new ArgumentOutOfRangeException(nameof(stopBits))
    };
}