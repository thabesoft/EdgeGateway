using System.Net.Sockets;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Transport;


internal sealed class TcpTransportConnection(IpTransportDescriptor descriptor) : LifecycleStateMachine, ITransportConnection
{
    private readonly TcpClient _tcpClient = new();


    public ConnectionEndpoint Endpoint { get; } = descriptor.Endpoint;


    public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) =>
        _tcpClient.GetStream().ReadAsync(buffer, cancellationToken);
    public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default) =>
        _tcpClient.GetStream().WriteAsync(data, cancellationToken);


    protected override ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        return _tcpClient.ConnectAsync(descriptor.IPAddress, descriptor.Port, cancellationToken);
    }
    protected override ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        _tcpClient.Close();
        return ValueTask.CompletedTask;
    }
    protected override ValueTask OnDisposeAsync()
    {
        _tcpClient.Dispose();
        return ValueTask.CompletedTask;
    }
}
