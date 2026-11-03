using ThabeSoft.EdgeGateway.Ports;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway;


internal sealed class Gateway(IPortFactory portFactory) : ResilientLifecycleBase, IGateway
{
    private readonly ConcurrentDictionary<PortId, IPort> _ports = [];
    public IReadOnlyDictionary<PortId, IPort> Ports => _ports.AsReadOnly();



    public IPort AddPort(IPortDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        var port = portFactory.Create(descriptor);
        return _ports[port.Id] = port;
    }

    public async ValueTask RemovePortAsync(PortId id, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(id);

        if (!_ports.TryRemove(id, out var port)) return;
        await port.CloseAsync(cancellationToken);
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        foreach (var i in _ports.Values)
        {
            try
            {
                await i.OpenAsync(cancellationToken);
            }
            catch (Exception ex)
            {

            }
        }
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        foreach (var i in _ports.Values)
        {
            try
            {
                await i.CloseAsync(cancellationToken);
            }
            catch
            {

            }
        }
    }
    protected override async ValueTask OnDisposeAsync()
    {
        foreach (var i in _ports.Values)
        {
            await i.DisposeAsync();
        }
    }
}