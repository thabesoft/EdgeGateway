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
    private readonly AsyncGate _reconnectGate = new();

    private readonly RetryBackoff _retryBackoff;
    private readonly IpTransportDescriptor _descriptor;
    private readonly ILogger<TcpTransportConnection> _logger;
    private readonly IDisposableLifecycle _reconnectBackgroundLifecycle;



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
        {
            _reconnectGate.Open();
            throw new ArgumentNullException($"TCP({Endpoint})连接未建立，无法读取");
        }

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
            _reconnectGate.Open();
            throw;
        }
    }
    public async ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
    {
        if (_tcpClient?.Connected != true)
        {
            _reconnectGate.Open();
            throw new ArgumentNullException($"TCP({Endpoint})连接未建立，无法写入");
        }

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
            _reconnectGate.Open();
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
        _reconnectGate.Open();
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
            await _reconnectGate.WaitAsync(cancellationToken);
            _reconnectGate.Close();

            if (cancellationToken.IsCancellationRequested)
                break;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _tcpClient?.Close();
                    _tcpClient?.Dispose();

                    _tcpClient = new();
                    await _tcpClient.ConnectAsync(_descriptor.IP, _descriptor.Port, cancellationToken);
                    _logger.LogInformation("TCP({Endpoint})客户端已连接", Endpoint);

                    _retryBackoff.Reset();
                    break;
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    var time = _retryBackoff.GetNextInterval();
                    _logger.LogWarning("TCP({Endpoint}) 连接失败，({Time})秒后重试: {Msg}", Endpoint, time.TotalSeconds, ex.Message);
                    await Task.Delay(time, cancellationToken);
                }
            }
        }
    }
}


/// <summary>
/// 门闩锁
/// </summary>
public class AsyncGate : IDisposable
{
    private readonly object _sync = new();
    private bool _open;
    private TaskCompletionSource<bool> _openedTcs = new(
        TaskCreationOptions.RunContinuationsAsynchronously);


    public void Open()
    {
        TaskCompletionSource<bool>? toRelease = null;
        lock (_sync)
        {
            if (_open) return;
            _open = true;
            toRelease = _openedTcs;
        }
        toRelease.TrySetResult(true);
    }

    public void Close()
    {
        lock (_sync)
        {
            if (!_open) return;
            _open = false;
            _openedTcs = new TaskCompletionSource<bool>(
                TaskCreationOptions.RunContinuationsAsynchronously);
        }
    }


    public async Task WaitAsync(CancellationToken cancellationToken = default)
    {
        Task waitTask;
        lock (_sync)
        {
            if (_open) return;
            waitTask = _openedTcs.Task;
        }

        await waitTask.WaitAsync(cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        lock (_sync)
        {
            _openedTcs.TrySetCanceled();
        }
    }
}