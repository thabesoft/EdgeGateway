using EdgeGateway.Channels;
using EdgeGateway.Ports;
using System.Collections.Concurrent;

namespace EdgeGateway;


public sealed class Gateway(IPortFactory portFactory) : IAsyncDisposable
{
    private readonly ConcurrentDictionary<PortId, IPort> _ports = [];


    public async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        foreach (var i in _ports.Values)
        {
            if (i.IsOpen) continue;
            await i.OpenAsync(cancellationToken);
        }
    }
    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        foreach (var i in _ports.Values)
        {
            if (!i.IsOpen) continue;
            await i.CloseAsync(cancellationToken);
        }
    }



    public IPort AddPort(IChannelDescriptor channelDescriptor)
    {
        var port = portFactory.Create(channelDescriptor);
        return _ports[port.Id] = port;
    }

    public async ValueTask RemovePortAsync(PortId id, CancellationToken cancellationToken = default)
    {
        if (!_ports.TryRemove(id, out var port)) return;
        await port.CloseAsync(cancellationToken);
    }


    public async ValueTask DisposeAsync()
    {
        foreach(var i in _ports.Values)
        {
            await i.DisposeAsync();
        }
    }
}