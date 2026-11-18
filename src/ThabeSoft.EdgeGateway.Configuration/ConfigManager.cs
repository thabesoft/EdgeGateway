using Microsoft.Extensions.Logging;
using System.Collections;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Configuration.Models;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.EdgeGateway.Configuration;


internal sealed class ConfigManager(IGateway gateway, ILogger<ConfigManager> logger)
{
    private GatewayConfig _current = new();


    public async Task ApplyAsync(GatewayConfig newConfig, CancellationToken cancellationToken = default)
    {
        var oldPorts = _current.Ports.ToDictionary(p => p.Descriptor.Id);
        var newPorts = newConfig.Ports.ToDictionary(p => p.Descriptor.Id);


        // 删除：旧的有，新的没有
        foreach (var (id, _) in oldPorts)
        {
            if (!newPorts.ContainsKey(id))
            {
                try { await gateway.RemovePortAsync(id, cancellationToken); }
                catch (Exception ex) { logger.LogError(ex, "移除端口 {Id} 失败", id); }
            }
        }

        // 新增：新的有，旧的没有
        foreach (var (id, portCfg) in newPorts)
        {
            if (!oldPorts.ContainsKey(id))
            {
                try { await AddPortRecursiveAsync(portCfg, cancellationToken); }
                catch (Exception ex) { logger.LogError(ex, "新增端口 {Id} 失败", id); }
            }
        }

        // 修改：两边都有，但内容不同
        foreach (var (id, newPortCfg) in newPorts)
        {
            if (oldPorts.TryGetValue(id, out var oldPortCfg) && oldPortCfg != newPortCfg)
            {
                try
                {
                    await gateway.RemovePortAsync(id, cancellationToken);
                    await AddPortRecursiveAsync(newPortCfg, cancellationToken);
                }
                catch (Exception ex) { logger.LogError(ex, "重建端口 {Id} 失败", id); }
            }
        }

        // 4. 全部成功后，更新快照
        _current = newConfig;
    }

    private async Task AddPortRecursiveAsync(PortConfig portCfg, CancellationToken cancellationToken)
    {
        var port = await gateway.AddPortAsync(portCfg.Descriptor, cancellationToken);
        foreach (var deviceCfg in portCfg.Devices)
        {
            var device = await port.AddDeviceAsync(deviceCfg.Descriptor, cancellationToken);

            foreach (var subCfg in deviceCfg.Subscriptions)
            {
                switch (subCfg.ValueType)
                {
                    case CodecKind.Bool:
                        await device.AddSubscriptionAsync((SubscriptionDescriptor<bool>)subCfg.Descriptor, cancellationToken);
                        break;
                    case CodecKind.Bits:
                        await device.AddSubscriptionAsync((SubscriptionDescriptor<BitArray>)subCfg.Descriptor, cancellationToken);
                        break;
                    case CodecKind.Byte:
                        await device.AddSubscriptionAsync((SubscriptionDescriptor<byte>)subCfg.Descriptor, cancellationToken);
                        break;
                    default: throw new InvalidOperationException("不支持的值类型");
                }

            }
        }
    }
}