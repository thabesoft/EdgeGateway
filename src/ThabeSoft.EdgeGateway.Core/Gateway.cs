using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


internal sealed class Gateway(
        PortFactory portFactory,
        ILogger<Gateway> logger
    ) : RetryableLifecycle(logger), IGateway
{
    private readonly ConcurrentDictionary<ConnectionEndpoint, IPort> _ports = [];
    public IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports => _ports;


    public IPort AddPort(ITransportDescriptor transportDescriptor)
    {
        if (_ports.ContainsKey(transportDescriptor.Endpoint))
            throw new InvalidOperationException($"端口添加失败, 存在相同的 端点({transportDescriptor.Endpoint})");

        var port = portFactory.Create(transportDescriptor);
        _ports[port.Endpoint] = port;

        LogAddPort(transportDescriptor.Endpoint);
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

        if (logger.IsEnabled(LogLevel.Debug))
            logger.LogDebug("网关释放");
    }



    private void LogAddPort(ConnectionEndpoint endpoint)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        logger.LogDebug("端口已添加 [{Endpoint}]", endpoint);

    }
}