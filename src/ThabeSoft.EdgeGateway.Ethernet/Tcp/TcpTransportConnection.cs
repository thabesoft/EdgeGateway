using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Primitives;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Ethernet.Tcp;


internal sealed class TcpTransportConnection : ITransportConnection
{
    private readonly DisposeFlag _disposeFlag = new();

    private readonly ILifecycleStateMachine _stateMachine;
    private TcpClient? _tcpClient;
    private readonly AsyncGate _reconnectGate = new();

    private readonly RetryBackoff _retryBackoff;
    private readonly IpTransportDescriptor _descriptor;
    private readonly ILogger<TcpTransportConnection> _logger;
    private readonly ILifecycle _reconnectBackgroundLifecycle;



    public TcpTransportConnection(
            IpTransportDescriptor descriptor,
            ILifecycleFactory lifecycleFactory,
            ILifecycleStateMachineFactory lifecycleStateMachineFactory,
            ILogger<TcpTransportConnection> logger)
    {
        _logger = logger;
        _descriptor = descriptor;
        _retryBackoff = new(descriptor.Retry);
        _stateMachine = lifecycleStateMachineFactory.Create(descriptor.StateMachine);
        _reconnectBackgroundLifecycle = lifecycleFactory.Create(LifecycleBuilder.Background(ReconnectLoopAsync));
    }

    public ConnectionEndpoint Endpoint => _descriptor.Endpoint;
    public LifecycleState State => _stateMachine.State;


    public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);

        if (_tcpClient?.Connected != true)
        {
            _reconnectGate.Open();
            throw new ArgumentNullException($"TCP({Endpoint})连接未建立，无法读取");
        }

        try
        {
            return await _stateMachine.ExecuteAsync
            (
                handler: ct => _tcpClient.GetStream().ReadAsync(buffer, ct),
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
        _disposeFlag.ThrowIfDisposed(this);

        if (_tcpClient?.Connected != true)
        {
            _reconnectGate.Open();
            throw new ArgumentNullException($"TCP({Endpoint})连接未建立，无法写入");
        }

        try
        {
            await _stateMachine.ExecuteAsync
            (
                handler: ct => _tcpClient.GetStream().WriteAsync(data, ct),
                cancellationToken: cancellationToken
            );
        }
        catch (Exception ex) when (ex is IOException or TimeoutException or SocketException)
        {
            _reconnectGate.Open();
            throw;
        }
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return _stateMachine.TransitionToRunningAsync(OnStartAsync, cancellationToken);
    }
    public ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        _disposeFlag.ThrowIfDisposed(this);
        return _stateMachine.TransitionToStoppedAsync(OnStopAsync, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (!_disposeFlag.TryMarkDisposed()) return;

        try { await OnStopAsync(CancellationToken.None); }
        catch (Exception ex) { _logger.LogError(ex, "Tcp 传输停止失败"); }

        await Safe.DisposeSafelyAsync(_tcpClient, _logger);
        await Safe.DisposeSafelyAsync(_reconnectBackgroundLifecycle, _logger);
        await Safe.DisposeSafelyAsync(_stateMachine, _logger);
    }


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