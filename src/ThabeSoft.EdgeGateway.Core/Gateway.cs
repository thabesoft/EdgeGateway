using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;

internal sealed class Gateway(
        PortManageer portManageer
    ) : RetryableLifecycle, IGateway
{
    private readonly ConcurrentDictionary<ConnectionEndpoint, IPort> _ports = [];
    public IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports => _ports;

    public async ValueTask<IPort> AddPortAsync(ITransportDescriptor transportDescriptor)
    {
        var port = await portManageer.RegisterAsync(transportDescriptor);
        _ports[port.Endpoint] = port;
        return port;
    }

    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        List<Exception> exceptions = [];

        foreach (var port in Ports.Values)
        {
            try
            {
                await port.StartAsync(cancellationToken);
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

        foreach (var port in Ports.Values)
        {
            try
            {
                await port.StopAsync(cancellationToken);
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
            await port.DisposeAsync();

        _ports.Clear();
    }
}