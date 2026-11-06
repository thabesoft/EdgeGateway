using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;

internal sealed class Gateway(
        PortManageer portManageer,
        ILogger<Gateway> logger
    ) : RetryableLifecycle, IGateway
{
    private readonly ConcurrentDictionary<ConnectionEndpoint, IPort> _ports = [];
    public IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports => _ports;

    public IPort AddPort(ITransportDescriptor transportDescriptor)
    {
        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("网关添加端口 [{Endpoint}]", transportDescriptor.Endpoint);


        var port = portManageer.Register(transportDescriptor);
        _ports[port.Endpoint] = port;
        return port;
    }

    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("网关启动");

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

        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("网关停止");

        if (exceptions.Count == 0) return;
        throw new AggregateException("部分端口关闭失败", exceptions);
    }
    protected override async ValueTask OnDisposeAsync()
    {
        foreach (var port in _ports.Values)
            await port.DisposeAsync();

        _ports.Clear();

        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("网关释放");
    }
}