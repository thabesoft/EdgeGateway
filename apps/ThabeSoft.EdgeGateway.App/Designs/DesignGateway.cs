using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.Designs;


public sealed class DesignGateway : IGateway
{
    private readonly ConcurrentDictionary<ChannelAddress, IPort> _ports = [];

    public LifecycleState State { get; private set; }
    public IReadOnlyDictionary<ChannelAddress, IPort> Ports => _ports.AsReadOnly();


    public async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        State = LifecycleState.Starting;
        foreach (var i in _ports.Values) await i.OpenAsync(cancellationToken);
        State = LifecycleState.Running;
    }
    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        State = LifecycleState.Stopping;
        foreach (var i in _ports.Values) await i.CloseAsync(cancellationToken);
        State = LifecycleState.Stopped;
    }

    public IPort AddPort(IPortDescriptor descriptor)
    {
        var port = new DesignPort(descriptor, this);
        _ports[port.ChannelAddress] = port;
        return port;
    }
    public async ValueTask RemovePortAsync(ChannelAddress channelAddress, CancellationToken cancellationToken = default)
    {
        if (!_ports.TryRemove(channelAddress, out var port)) return;
        await port.DisposeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var i in _ports.Values) await i.DisposeAsync();
        _ports.Clear();
        State = LifecycleState.Disposed;
    }
}