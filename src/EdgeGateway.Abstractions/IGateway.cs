using EdgeGateway.Bytes;
using EdgeGateway.Ports;

namespace EdgeGateway;


public interface IGateway : IAsyncDisposable
{
    /// <summary>
    /// 指示网关当前是否正在运行。
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// 所有端口
    /// </summary>
    IReadOnlyDictionary<IChannelAddress, IPort> Ports { get; }


    ValueTask StartAsync(CancellationToken cancellationToken = default);
    ValueTask StopAsync(CancellationToken cancellationToken = default);
}