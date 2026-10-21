using EdgeGateway.Bytes;
using EdgeGateway.Ports;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EdgeGateway;


/// <summary>
/// 根据配置自动更新的网关
/// </summary>
public sealed class Gateway(
        IPortManager portManager,
        IOptions<GatewayOptions> options,
        ILogger<Gateway> logger
    ) : IGateway
{
    public bool IsRunning { get; private set; }
    public IReadOnlyDictionary<IChannelAddress, IPort> Ports => portManager.Ports;


    public async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        if (IsRunning) return;

        var opened = new List<IPort>();
        try
        {
            foreach (var handle in Ports.Values.ToArray())
            {
                await OpenPortHandle(handle, cancellationToken);
            }

            IsRunning = true;
        }
        catch
        {
            await StartFailRollbackAsync(opened);
            throw;
        }
    }
    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        if (!IsRunning) return;

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(options.Value.PortCloseTimeout);

        var failures = new List<Exception>();
        foreach (var port in Ports.Values.ToArray())
        {
            try
            {
                await port.CloseAsync(timeoutCts.Token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "关闭端口 {Address} 失败", port.Descriptor.Address);
                failures.Add(ex);
            }
        }

        IsRunning = false;
        if (failures.Count > 0) throw new AggregateException("部分端口关闭失败", failures);
    }
    public async ValueTask DisposeAsync()
    {
        using var cts = new CancellationTokenSource(options.Value.PortCloseTimeout);

        try
        {
            await StopAsync(cts.Token);
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("停止网关超时（5s），部分端口可能未正常关闭，依赖进程退出清理");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "释放网关失败");
        }
    }




    // 失败回滚
    private async ValueTask StartFailRollbackAsync(List<IPort> opened)
    {
        foreach (var port in opened)
        {
            try
            {
                using var cts = new CancellationTokenSource(options.Value.StartRollbackPortCloseTimeout);
                await port.CloseAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning("启动回滚：端口 {Address} 关闭超时，依赖进程退出清理", port.Descriptor.Address);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "启动回滚时端口关闭失败");
            }
        }
    }
    // 打开端口
    private async ValueTask OpenPortHandle(IPort handle, CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(options.Value.PortOpenTimeout);

        try
        {
            await handle.OpenAsync(cts.Token);
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"端口 {handle.Descriptor.Address} 打开超时");
        }
    }
}


public sealed class GatewayOptions
{
    /// <summary>
    /// 启动回滚端口关闭超时时长
    /// </summary>
    public TimeSpan StartRollbackPortCloseTimeout { get; init; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// 开启端口超时时长
    /// </summary>
    public TimeSpan PortOpenTimeout { get; init; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// 关闭端口超时时长
    /// </summary>
    public TimeSpan PortCloseTimeout { get; init; } = TimeSpan.FromSeconds(5);
}