using EdgeGateway.Bytes;
using Microsoft.Extensions.Options;

namespace EdgeGateway.Ports;


internal sealed class PortManager(
        IEnumerable<IPortFactory> portFactories,
        IOptions<PortManagerOptions> options
    ) : IPortManager
{
    private readonly Dictionary<IChannelAddress, IPort> _handles = [];

    public IReadOnlyDictionary<IChannelAddress, IPort> Ports => _handles;


    public IPort Add(ChannelDescriptor descriptor)
    {
        if (_handles.ContainsKey(descriptor.Address))
            throw new InvalidOperationException("端口已存在");

        var factory = portFactories.FirstOrDefault(x => x.CanCreate(descriptor)) ??
            throw new InvalidOperationException("没有工厂能处理该配置");

        return _handles[descriptor.Address] = factory.Create(descriptor);
    }

    public async ValueTask RemoveAsync(IChannelAddress address, CancellationToken cancellationToken = default)
    {
        if (!_handles.TryGetValue(address, out var portHandle))
            return;

        using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(options.Value.PortCloseTimeout);

        try
        {
            await portHandle.CloseAsync(timeoutCts.Token);
            _handles.Remove(portHandle.Address);
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"端口 {address} 关闭超时");
        }
    }
}

public sealed class PortManagerOptions
{
    /// <summary>
    /// 关闭端口超时时长
    /// </summary>
    public TimeSpan PortCloseTimeout { get; init; } = TimeSpan.FromSeconds(5);
}