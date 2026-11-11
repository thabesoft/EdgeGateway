using Microsoft.Extensions.Logging;
using RJCP.IO.Ports;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Transport.Byte;

using RJCPStopBit = RJCP.IO.Ports.StopBits;
using RJCPParity = RJCP.IO.Ports.Parity;


namespace ThabeSoft.EdgeGateway.Serial.Internal;


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
            parity: Parse(descriptor.Parity),
            stopbits: Parse(descriptor.StopBits));

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