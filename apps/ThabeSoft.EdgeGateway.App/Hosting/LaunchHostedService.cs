using Microsoft.Extensions.Hosting;
using System.Net;
using ThabeSoft.EdgeGateway.App.Features.Gateway;
using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.App.Hosting;


internal sealed class LaunchHostedService(GatewayViewModel gateway) :
    IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2333), gateway, cancellationToken);
        await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2334), gateway, cancellationToken);

        await gateway.StartCommand.ExecuteAsync(null);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async Task AddPortAsync(IpTransportDescriptor transportDescriptor, GatewayViewModel gateway, CancellationToken cancellationToken)
    {
        var port = await gateway.AddPortAsync(new() { Transport = transportDescriptor }, cancellationToken);

        const int count = 2;

        for (byte i = 1; i <= count; i++)
            await AddDeviceAsync(i, port, cancellationToken);
    }
    private static async Task AddDeviceAsync(byte id, PortViewModel port, CancellationToken cancellationToken)
    {
        var device = await port.AddDeviceAsync(new() { Protocol = Modbus.Ip }, cancellationToken);

        var trigger = Trigger.Interval(100);

        for (int i = 0; i < 10; i++)
        {
            // Double 占 4 个寄存器，地址按 4 递增
            var tag = Modbus.HoldingRegister(id, (ushort)(i * 4), Codec.Double);
            // 网关订阅
            await device.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag, trigger), cancellationToken);
        }
    }
}