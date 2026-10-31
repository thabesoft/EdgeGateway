using ThabeSoft.EdgeGateway.Port;
using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway;


internal sealed class Gateway(IPortFactory portFactory) : IGateway
{
    private readonly ConcurrentDictionary<PortId, IPort> _ports = [];
    public IReadOnlyDictionary<PortId, IPort> Ports => _ports.AsReadOnly();


    public bool IsRunning { get; private set; }

    public async ValueTask StartAsync(CancellationToken cancellationToken = default)
    {
        if (IsRunning) return;
        IsRunning = true;


        foreach (var i in _ports.Values)
        {
            await i.OpenAsync(cancellationToken);
        }
    }
    public async ValueTask StopAsync(CancellationToken cancellationToken = default)
    {
        if (!IsRunning) return;
        IsRunning = false;

        foreach (var i in _ports.Values)
        {
            await i.CloseAsync(cancellationToken);
        }
    }



    public IPort AddPort(IPortDescriptor descriptor)
    {
        var port = portFactory.Create(descriptor);
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