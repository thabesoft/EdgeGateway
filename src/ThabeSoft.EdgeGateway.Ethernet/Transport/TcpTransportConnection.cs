using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Transport;


internal sealed class TcpTransportConnection(
        IpTransportDescriptor descriptor,
        ILogger<TcpTransportConnection> logger
    ) : ITransportConnection
{
    private readonly LifecycleStateMachineA _stateMachine = Lifecycle.Normal(x => x.StartStop(15), logger: logger);
    private readonly TcpClient _tcpClient = new();


    public ConnectionEndpoint Endpoint { get; } = descriptor.Endpoint;
    public LifecycleState State => _stateMachine.State;



    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (_tcpClient?.Connected != true)
            throw new InvalidOperationException($"TCP 连接未建立，无法写入: {Endpoint}");

        return _stateMachine.ExecuteAsync
        (
            state: (TcpClient: _tcpClient, Buffer: buffer),
            action: static (state, ct) => state.TcpClient.GetStream().ReadAsync(state.Buffer, ct),
            cancellationToken: cancellationToken
        );
    }
    public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        if (_tcpClient?.Connected != true)
            throw new InvalidOperationException($"TCP 连接未建立，无法写入: {Endpoint}");

        return _stateMachine.ExecuteAsync
        (
            state: (TcpClient: _tcpClient, Data: data),
            action: static (state, ct) => state.TcpClient.GetStream().WriteAsync(state.Data, ct),
            cancellationToken: cancellationToken
        );
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);



    private ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        return _tcpClient.ConnectAsync(descriptor.IPAddress, descriptor.Port, cancellationToken);
    }
    private ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        _tcpClient.Close();
        return ValueTask.CompletedTask;
    }
    private ValueTask OnDisposeAsync()
    {
        _tcpClient.Dispose();
        return ValueTask.CompletedTask;
    }
}
