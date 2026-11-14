using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net.Sockets;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;

namespace ThabeSoft.EdgeGateway.Transport.Tcp;


internal sealed class TcpTransportConnection : ITransportConnection
{
    private readonly ILifecycleStateMachine _stateMachine;
    private TcpClient? _tcpClient;
    private readonly SemaphoreSlim _reconnectSignal = new(0, 1);
    private readonly RetryBackoff _retryBackoff;
    private readonly IpTransportDescriptor _descriptor;
    private readonly ILogger<TcpTransportConnection> _logger;
    private IDisposableLifecycle _reconnectBackgroundLifecycle;

    public TcpTransportConnection(
            IpTransportDescriptor descriptor,
            ILogger<TcpTransportConnection>? logger = null)
    {
        _descriptor = descriptor;
        _logger = logger ?? NullLogger<TcpTransportConnection>.Instance;

        _stateMachine = Lifecycle.StrictStateMachine(logger);
        _retryBackoff = new(descriptor.Retry);
        _reconnectBackgroundLifecycle = Lifecycle.Background(ReconnectLoopAsync, logger);
    }

    public ConnectionEndpoint Endpoint => _descriptor.Endpoint;
    public LifecycleState State => _stateMachine.State;



    public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (_tcpClient?.Connected != true)
            throw new InvalidOperationException($"TCP 连接未建立，无法写入: {Endpoint}");

        try
        {
            return await _stateMachine.ExecuteAsync
            (
                method: ct => _tcpClient.GetStream().ReadAsync(buffer, ct),
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex) when (ex is IOException or TimeoutException or SocketException)
        {
            _reconnectSignal.Release();
            throw;
        }
    }
    public async ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        if (_tcpClient?.Connected != true)
            throw new InvalidOperationException($"TCP 连接未建立，无法写入: {Endpoint}");

        try
        {
            await _stateMachine.ExecuteAsync
            (
                method: ct => _tcpClient.GetStream().WriteAsync(data, ct),
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex) when (ex is IOException or TimeoutException or SocketException)
        {
            _reconnectSignal.Release();
            throw;
        }
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);



    private async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (_tcpClient?.Connected == true) return;

        await _reconnectBackgroundLifecycle.StartAsync(cancellationToken);
        _reconnectSignal.Release();
    }
    private async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        await _reconnectBackgroundLifecycle.StopAsync(cancellationToken);
    }
    private ValueTask OnDisposeAsync()
    {
        return OnStopAsync(CancellationToken.None);
    }


    private async Task ReconnectLoopAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await _reconnectSignal.WaitAsync(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    await _stateMachine.StartAsync(async ct =>
                    {
                        _tcpClient?.Close();
                        _tcpClient?.Dispose();

                        _tcpClient = new();
                        await _tcpClient.ConnectAsync(_descriptor.IP, _descriptor.Port, ct);

                    }, cancellationToken);
                }
                catch (Exception ex)
                {
                    var time = _retryBackoff.GetNextInterval();
                    _logger.LogWarning("TCP 连接失败，(Time)秒后重试: {Msg}", time.TotalSeconds, ex.Message);
                    await Task.Delay(time, cancellationToken);
                }
            }
        }
    }
}
