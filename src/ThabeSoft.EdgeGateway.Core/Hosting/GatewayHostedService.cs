using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Hosting;


internal sealed class GatewayHostedService(IGateway gateway, GatewayOptions options, ILogger<GatewayHostedService> loggeer) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 根据配置初始化
        foreach (var options in options.Ports)
            await AddPort(gateway, options);

        if (options.AutoLaunch)
            await gateway.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await gateway.StopAsync(cancellationToken);
    }


    public async ValueTask AddPort(IGateway gateway, PortOptions options)
    {
        try
        {
            var port = await gateway.AddPortAsync(options.Transport);

            foreach (var device in options.Devices)
                await AddDevice(port, device);
        }
        catch (Exception ex)
        {
            loggeer.LogError(ex, "端口添加失败 [{Addr}]",
                options.Transport.Endpoint);
        }
    }

    public async ValueTask AddDevice(IPort port, DeviceOptions options)
    {
        try
        {
            var device = await port.AddDeviceAsync(options.Descriptor);

            foreach (var tag in options.Subscriptions)
                await AddSubscriptionAsync(device, tag);
        }
        catch (Exception ex)
        {
            loggeer.LogError(ex, "设备添加失败 [{Id}], 所属通道地址:{Addr}",
                options.Descriptor.DeviceId, port.Endpoint);
        }
    }

    public async ValueTask AddSubscriptionAsync(IDevice device, ISubscriptionOptions subscriptionOptions)
    {
        try
        {
            await subscriptionOptions.AddSubscriptionAsync(device);
        }
        catch (Exception ex)
        {
            loggeer.LogError(ex, "标签添加失败 [{Name}], 所属设备Id: {Id}",
                subscriptionOptions, device.Id);
        }
    }
}