using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Options;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway;


internal sealed class Gateway(IPortFactory portFactory, ILogger<Gateway> logger) : ResilientLifecycleBase, IGateway
{
    private readonly ConcurrentDictionary<ChannelAddress, IPort> _ports = [];
    public IReadOnlyDictionary<ChannelAddress, IPort> Ports => _ports.AsReadOnly();


    public IPort AddPort(IPortDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        var port = portFactory.Create(descriptor);
        return _ports[port.ChannelAddress] = port;
    }

    public async ValueTask RemovePortAsync(ChannelAddress channelAddress, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(channelAddress);

        if (!_ports.TryRemove(channelAddress, out var port)) return;
        await port.CloseAsync(cancellationToken);
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        List<Exception> exceptions = [];

        foreach (var port in _ports.Values)
        {
            try
            {
                await port.OpenAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count == 0) return;
        throw new AggregateException("部分端口开启失败", exceptions);
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        List<Exception> exceptions = [];

        foreach (var port in _ports.Values)
        {
            try
            {
                await port.CloseAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count == 0) return;
        throw new AggregateException("部分端口关闭失败", exceptions);
    }

    protected override async ValueTask OnDisposeAsync()
    {
        foreach (var port in _ports.Values)
        {
            try
            {
                await port.DisposeAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "端口释放失败");
            }
        }
    }
}