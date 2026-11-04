using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Options;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Hosting;


internal sealed class GatewayHostedService(IGateway gateway, GatewayOptions options, ILogger<GatewayHostedService> loggeer) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // 根据配置初始化
        foreach (var options in options.Ports)
            AddPort(gateway, options);

        if (options.AutoLaunch)
            await gateway.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await gateway.StopAsync(cancellationToken);
    }


    public void AddPort(IGateway gateway, PortOptions options)
    {
        try
        {
            var port = gateway.AddPort(options.Transport);

            foreach (var device in options.Devices)
                AddDevice(port, device);
        }
        catch (Exception ex)
        {
            loggeer.LogError(ex, "端口添加失败 [{Addr}]",
                options.Transport.Address);
        }
    }

    public void AddDevice(IPort port, DeviceOptions options)
    {
        try
        {
            var device = port.AddDevice(options.Descriptor);

            foreach (var tag in options.Tags)
                AddTag(device, tag);
        }
        catch (Exception ex)
        {
            loggeer.LogError(ex, "设备添加失败 [{Id}], 所属通道地址:{Addr}",
                options.Descriptor.DeviceId, port.Endpoint);
        }
    }

    public void AddTag(IDevice device, ITagOptions tagOptions)
    {
        try
        {
            tagOptions.RegisterTo(device);
        }
        catch (Exception ex)
        {
            loggeer.LogError(ex, "标签添加失败 [{Name}], 所属设备Id: {Id}",
                tagOptions.Name, device.Id);
        }
    }
}