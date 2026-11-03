using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Options;

namespace ThabeSoft.EdgeGateway.Hosting;


internal sealed class GatewayHostedService(IGateway gateway, GatewayOptions options, ILogger<GatewayHostedService> loggeer) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Initialize(options);
        await gateway.StartAsync(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await gateway.StopAsync(cancellationToken);
    }


    public void Initialize(GatewayOptions options)
    {
        foreach (var portOptions in options.Ports)
        {
            try
            {
                var port = gateway.AddPort(portOptions.Descriptor);
                foreach (var deviceOptions in portOptions.Devices)
                {
                    try
                    {
                        var device = port.AddDevice(deviceOptions.Descriptor);
                        //device.Register()
                    }
                    catch (Exception ex)
                    {
                        loggeer.LogError(ex, "设备配置失败 [{Id}], 所属通道地址:{Addr}",
                            deviceOptions.Descriptor.DeviceId,
                            portOptions.Descriptor.ChannelDescriptor.Address);
                    }
                }
            }
            catch (Exception ex)
            {
                loggeer.LogError(ex, "端口配置失败, 通道地址: {Addr}", portOptions.Descriptor.ChannelDescriptor.Address);
            }
        }
    }
}