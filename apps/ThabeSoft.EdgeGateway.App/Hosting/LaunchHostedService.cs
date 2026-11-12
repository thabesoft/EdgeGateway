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
        var trigger = Trigger.Interval(5000);
        var tag1 = Modbus.HoldingRegister(1, 0, Codec.Double);
        var tag2 = Modbus.HoldingRegister(1, 4, Codec.Double);
        var tag3 = Modbus.HoldingRegister(1, 8, Codec.Double);
        var tag4 = Modbus.HoldingRegister(1, 12, Codec.Double);

        // 从站模拟
        var network = simulator.AddPort(Simulation.ModbusIpPort(transportDescriptor));
        var slave = await network.AddDeviceAsync(Simulation.ModbusDevice(1), cancellationToken);
        slave.Value(tag1, trigger, Simulation.SineWaveValue(1, 0.2));
        slave.Value(tag2, trigger, Simulation.SineWaveValue(2, 0.4));
        //slave.Value(tag3, trigger, Simulation.SineWaveValue(3, 0.6));
        //slave.Value(tag4, trigger, Simulation.SineWaveValue(4, 0.8));


        // 网关默认数据
        var port = await gateway.AddPortAsync(Ethernet.Tcp(transportDescriptor.IPEndPoint), cancellationToken);
        var dvice = await port.AddDeviceAsync(DeviceDescriptor.Create(Modbus.Ip), cancellationToken);
        await dvice.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag1, trigger), cancellationToken);
        await dvice.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag2, trigger), cancellationToken);
        //await dvice.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag3, trigger), cancellationToken);
        //await dvice.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag4, trigger), cancellationToken);


        await Task.Delay(10, cancellationToken);
        await simulator.StartAsync(cancellationToken);
        await Task.Delay(2000, cancellationToken);
        await gateway.StartCommand.ExecuteAsync(null);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}