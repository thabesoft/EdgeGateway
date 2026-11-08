using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway;


internal sealed class Gateway(
        PortFactory portFactory,
        ILogger<Gateway> logger
    ) : IGateway
{
    private readonly LifecycleStateMachineA _stateMachine = Lifecycle.Normal(x => x.Start(120).Stop(30), logger: logger);
    private readonly ConcurrentDictionary<ConnectionEndpoint, IPort> _ports = [];


    public IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports => _ports;
    public LifecycleState State => _stateMachine.State;



    public IPort AddPort(ITransportDescriptor transportDescriptor)
    {
        return _stateMachine.Execute(() =>
        {
            if (_ports.ContainsKey(transportDescriptor.Endpoint))
                throw new InvalidOperationException($"端口添加失败, 存在相同的 端点({transportDescriptor.Endpoint})");

            var port = portFactory.Create(transportDescriptor, this);
            _ports[port.Endpoint] = port;

            LogAddPort(transportDescriptor.Endpoint);
            return port;
        });
    }


    public ValueTask StartAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StartAsync(OnStartAsync, cancellationToken);
    public ValueTask StopAsync(CancellationToken cancellationToken = default) =>
        _stateMachine.StopAsync(OnStopAsync, cancellationToken);
    public ValueTask DisposeAsync() =>
        _stateMachine.DisposeAsync(OnDisposeAsync);


    private async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        foreach (var port in Ports.Values)
        {
            try
            {
                await port.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                logger.LogError(ex, "端口 {Endpoint} 启动失败", port.Endpoint);
            }
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"网关启动时 {errors.Count} 个端口启动失败", errors);
    }
    private async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        List<Exception> errors = [];

        foreach (var port in Ports.Values)
        {
            try
            {
                await port.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                logger.LogError(ex, "端口 {Endpoint} 停止失败", port.Endpoint);
            }
        }

        if (errors.Count == 0) return;
        throw new AggregateException($"网关停止时 {errors.Count} 个端口停止失败", errors);
    }
    private async ValueTask OnDisposeAsync()
    {
        var ports = _ports.Values.ToArray();
        _ports.Clear();

        foreach (var port in ports)
        {
            try { await port.DisposeAsync(); }
            catch (Exception ex)
            {
                logger.LogError(ex, "端口 {Endpoint} 释放失败", port.Endpoint);
            }
        }
    }



    private void LogAddPort(ConnectionEndpoint endpoint)
    {
        if (!logger.IsEnabled(LogLevel.Debug)) return;
        logger.LogDebug("端口已添加 [{Endpoint}]", endpoint);
    }
}