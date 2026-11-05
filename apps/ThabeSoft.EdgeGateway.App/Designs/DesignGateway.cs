using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.App.Designs;


public sealed class DesignGateway : RetryableLifecycle, IGateway
{
    private readonly ConcurrentDictionary<ConnectionEndpoint, IPort> _ports = [];
    public IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports => _ports.AsReadOnly();


    public IPort AddPort(ITransportDescriptor transportDescriptor)
    {
        var port = new DesignPort(transportDescriptor, this);
        _ports[port.Endpoint] = port;
        return port;
    }
    public async ValueTask RemovePortAsync(ConnectionEndpoint channelAddress, CancellationToken cancellationToken = default)
    {
        if (!_ports.TryRemove(channelAddress, out var port)) return;
        await port.DisposeAsync();
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        foreach (var i in _ports.Values) await i.StartAsync(cancellationToken);
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        foreach (var i in _ports.Values) await i.StopAsync(cancellationToken);
    }
    protected override async ValueTask OnDisposeAsync()
    {
        foreach (var i in _ports.Values) await i.DisposeAsync();
        _ports.Clear();
    }
}