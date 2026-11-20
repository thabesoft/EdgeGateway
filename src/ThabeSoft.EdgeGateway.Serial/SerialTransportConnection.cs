using Microsoft.Extensions.Logging;
using RJCP.IO.Ports;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.EdgeGateway.Transports.Byte;
using ThabeSoft.Primitives;
using RJCPParity = RJCP.IO.Ports.Parity;
using RJCPStopBit = RJCP.IO.Ports.StopBits;

namespace ThabeSoft.EdgeGateway.Serial;


internal sealed class SerialTransportConnection(
        SerialTransportDescriptor descriptor,
        ILifecycleStateMachine stateMachine,
        ILogger<SerialTransportConnection> logger,
        Func<ValueTask> onDispose
    ) : ITransportConnection, IReadBufferDiscardable
{
    private readonly DisposeFlag _disposeFlag = new();

    private readonly SerialPortStream _serialPort = new(
            port: descriptor.PortName,
            baud: descriptor.BaudRate,
            data: descriptor.DataBits,
            parity: Parse(descriptor.Parity),
            stopbits: Parse(descriptor.StopBits));

    public ConnectionEndpoint Endpoint { get; } = descriptor.Endpoint;
    public LifecycleState State => stateMachine.State;
    public Func<ValueTask>? DisposedCallback { get; init; }


    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        return stateMachine.ExecuteAsync
        (
            handler: ct => _serialPort.ReadAsync(buffer, ct),
            cancellationToken: cancellationToken
        );
    }
    public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        return stateMachine.ExecuteAsync
        (
            handler: ct => _serialPort.WriteAsync(data, ct),
            cancellationToken: cancellationToken
        );
    }
    public void DiscardInputBuffer()
    {
        stateMachine.Execute(_serialPort.DiscardInBuffer);
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        stateMachine.TransitionToRunningAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        stateMachine.TransitionToStoppedAsync(OnStopAsync, cancellationToken);
    public async ValueTask DisposeAsync()
    {
        if (!_disposeFlag.TryMarkDisposed()) return;

        await Safe.DisposeSafelyAsync(_serialPort, logger);
        await Safe.TryEexecuteAsync(onDispose, logger);
    }



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


    public static RJCPParity Parse(SerialParity parity) => parity switch
    {
        SerialParity.None => RJCPParity.None,
        SerialParity.Even => RJCPParity.Even,
        SerialParity.Odd => RJCPParity.Odd,
        SerialParity.Mark => RJCPParity.Mark,
        SerialParity.Space => RJCPParity.Space,
        _ => throw new ArgumentOutOfRangeException(nameof(parity))
    };

    public static RJCPStopBit Parse(SerialStopBits stopBits) => stopBits switch
    {
        SerialStopBits.One => RJCPStopBit.One,
        SerialStopBits.Two => RJCPStopBit.Two,
        SerialStopBits.OnePointFive => RJCPStopBit.One5,
        _ => throw new ArgumentOutOfRangeException(nameof(stopBits))
    };
}