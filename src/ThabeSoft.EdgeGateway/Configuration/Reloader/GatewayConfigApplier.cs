using Microsoft.Extensions.Logging;
using ThabeSoft.Configuration.Monitor;

namespace ThabeSoft.EdgeGateway.Configuration.Reloader;



internal sealed class GatewayConfigApplier
{
    private readonly IGateway _gateway;
    private readonly ILogger<GatewayConfigApplier> _logger;
    private GatewayConfig? _current;


    public GatewayConfigApplier(
            IGateway gateway,
            IConfigMonitor<GatewayConfig> monitor,
            ILogger<GatewayConfigApplier> logger
    )
    {
        _gateway = gateway;
        _logger = logger;

        monitor.OnChange(ApplyAsync);
    }

    private async ValueTask ApplyAsync(GatewayConfig config, CancellationToken cancellationToken = default)
    {
        if(_current is not null)
        {
            var oldPorts = _current.Ports.ToDictionary(p => p.Descriptor.Id);
            var newPorts = config.Ports.ToDictionary(p => p.Descriptor.Id);


            // 删除：旧的有，新的没有
            foreach (var (id, _) in oldPorts)
            {
                if (!newPorts.ContainsKey(id))
                {
                    try { await _gateway.RemovePortAsync(id, cancellationToken); }
                    catch (Exception ex) { _logger.LogError(ex, "移除端口 {Id} 失败", id); }
                }
            }

            // 新增：新的有，旧的没有
            foreach (var (id, portCfg) in newPorts)
            {
                if (!oldPorts.ContainsKey(id))
                {
                    try { await AddPortRecursiveAsync(portCfg, cancellationToken); }
                    catch (Exception ex) { _logger.LogError(ex, "新增端口 {Id} 失败", id); }
                }
            }

            // 修改：两边都有，但内容不同
            foreach (var (id, newPortCfg) in newPorts)
            {
                if (oldPorts.TryGetValue(id, out var oldPortCfg) && oldPortCfg != newPortCfg)
                {
                    try
                    {
                        await _gateway.RemovePortAsync(id, cancellationToken);
                        await AddPortRecursiveAsync(newPortCfg, cancellationToken);
                    }
                    catch (Exception ex) { _logger.LogError(ex, "重建端口 {Id} 失败", id); }
                }
            }
        }

        _current = config;
    }

    private async Task AddPortRecursiveAsync(PortConfig portConfig, CancellationToken cancellationToken)
    {
        var port = await _gateway.AddPortAsync(portConfig.Descriptor, cancellationToken);

        foreach (var deviceConfig in portConfig.Devices)
        {
            var device = await port.AddDeviceAsync(deviceConfig.Descriptor, cancellationToken);

            foreach (var subscriptionsConfig in deviceConfig.Subscriptions)
            {
                var descriptor = subscriptionsConfig.Descriptor;
                await descriptor.AddToDeviceAsync(device, cancellationToken);
            }
        }
    }
}
