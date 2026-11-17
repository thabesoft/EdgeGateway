using Microsoft.Extensions.Hosting;
using System.Net;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.App.Hosting;


internal sealed class LaunchHostedService(IGateway gateway) :
    IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        //await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2333), gateway, cancellationToken);
        ///await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2334), gateway, cancellationToken);

        //await gateway.StartAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async Task AddPortAsync(IpTransportDescriptor transportDescriptor, IGateway gateway, CancellationToken cancellationToken)
    {
        var port = await gateway.AddPortAsync(new() { Transport = transportDescriptor }, cancellationToken);

        const int count = 1;

        for (byte i = 1; i <= count; i++)
            await AddDeviceAsync(i, port, cancellationToken);
    }
    private static async Task AddDeviceAsync(byte id, IPort port, CancellationToken cancellationToken)
    {
        var device = await port.AddDeviceAsync(new() { Protocol = Modbus.Ip }, cancellationToken);

        var trigger = Trigger.Interval(100);

        for (int i = 0; i < 1; i++)
        {
            // Double 占 4 个寄存器，地址按 4 递增
            var tag = Modbus.HoldingRegister(id, (ushort)(i * 4), Codec.Double);
            // 网关订阅
            await device.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag, trigger), cancellationToken);
        }
    }
}