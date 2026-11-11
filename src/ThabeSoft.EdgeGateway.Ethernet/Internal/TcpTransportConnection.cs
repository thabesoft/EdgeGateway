using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using ThabeSoft.EdgeGateway.Ethernet;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ethernet.Internal;


internal sealed class TcpTransportConnection(
        IpTransportDescriptor descriptor,
        ILogger<TcpTransportConnection> logger
    ) : ITransportConnection
{
    private readonly IStrictLifecycleStateMachine _stateMachine = Lifecycle.StrictStateMachine(logger);
    private readonly TcpClient _tcpClient = new();


    public ConnectionEndpoint Endpoint { get; } = descriptor.Endpoint;
    public LifecycleState State => _stateMachine.State;



    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (_tcpClient?.Connected != true)
            throw new InvalidOperationException($"TCP 连接未建立，无法写入: {Endpoint}");

        return _stateMachine.ExecuteAsync
        (
            method: ct => _tcpClient.GetStream().ReadAsync(buffer, ct),
            cancellationToken: cancellationToken
        );
    }
    public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        if (_tcpClient?.Connected != true)
            throw new InvalidOperationException($"TCP 连接未建立，无法写入: {Endpoint}");

        return _stateMachine.ExecuteAsync
        (
            method: ct => _tcpClient.GetStream().WriteAsync(data, ct),
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
