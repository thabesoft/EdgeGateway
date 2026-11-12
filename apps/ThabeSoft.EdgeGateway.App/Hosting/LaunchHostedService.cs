using Microsoft.Extensions.Hosting;
using System.Net;
using ThabeSoft.EdgeGateway.App.Features.Gateway;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.App.Hosting;


internal sealed class LaunchHostedService(
    ISimulator simulator,
    GatewayViewModel gateway
    ) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var transportDescriptor = Ethernet.Tcp(IPAddress.Loopback, 1234);
        var tag = Modbus.HoldingRegister(1, 123, Codec.Double);
        var trigger = Trigger.Interval(2000);

        // 从站模拟
        var network = simulator.AddPort(Simulation.ModbusIpPort(transportDescriptor));
        var slave = await network.AddDeviceAsync(Simulation.ModbusDevice(1), cancellationToken);
        slave.Value(tag, trigger, Simulation.SineWaveValue(1, 0.2));


        // 网关默认数据
        var port = await gateway.AddPortAsync(Ethernet.Tcp(transportDescriptor.IPEndPoint), cancellationToken);
        await port.AddDeviceCommand.ExecuteAsync(DeviceDescriptor.Create(Modbus.Ip));

        var device = port.Devices.Values.First();
        await device.AddSubscriptionCommand.ExecuteAsync(SubscriptionDescriptor.Create(tag, trigger));


        await simulator.StartAsync(cancellationToken);
        await Task.Delay(1000, cancellationToken);
        await gateway.StartCommand.ExecuteAsync(null);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}