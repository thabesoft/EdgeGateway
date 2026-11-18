using Microsoft.Extensions.Logging;
using RJCP.IO.Ports;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.EdgeGateway.Transports.Byte;
using RJCPParity = RJCP.IO.Ports.Parity;
using RJCPStopBit = RJCP.IO.Ports.StopBits;

namespace ThabeSoft.EdgeGateway.Serial;


internal sealed class SerialTransportConnection(
        SerialTransportDescriptor descriptor,
        ILifecycleStateMachine stateMachine,
        ILogger<SerialTransportConnection> logger
    ) : ITransportConnection, IReadBufferDiscardable
{
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
        stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        stateMachine.DisposeAsync(OnDisposeAsync);



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
        try
        {
#if NETSTANDARD2_0
        _serialPort.Dispose();
#else
        await _serialPort.DisposeAsync();
#endif
        }
        catch(Exception ex)
        {
            logger.LogError(ex, "串口释放失败");
        }

        if (DisposedCallback is null) return;
        try { await DisposedCallback.Invoke(); }
        catch (Exception ex) { logger.LogError(ex, "串行传输释放回调执行异常"); }
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