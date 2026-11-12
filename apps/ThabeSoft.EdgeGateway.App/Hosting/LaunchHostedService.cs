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
        var transportDescriptor = Ethernet.Tcp(IPAddress.Loopback, 2333);
        var trigger = Trigger.Interval(10);
        var tag1 = Modbus.HoldingRegister(1, 0, Codec.Double);
        var tag2 = Modbus.HoldingRegister(1, 4, Codec.Double);
        var tag3 = Modbus.HoldingRegister(1, 8, Codec.Double);
        var tag4 = Modbus.HoldingRegister(1, 12, Codec.Double);
        var tag5 = Modbus.HoldingRegister(1, 16, Codec.Double);
        var tag6 = Modbus.HoldingRegister(1, 20, Codec.Double);
        var tag7 = Modbus.HoldingRegister(1, 24, Codec.Double);
        var tag8 = Modbus.HoldingRegister(1, 28, Codec.Double);

        // 从站模拟
        var network = simulator.AddPort(Simulation.ModbusIpPort(transportDescriptor));
        var slave = await network.AddDeviceAsync(Simulation.ModbusDevice(1), cancellationToken);
        // 1. 正弦波：振幅 1，step = 2π/20 ≈ 0.314，约 20 个采样点一个周期
        slave.Value(tag1, trigger, Simulation.SineWaveValue(1, 2 * Math.PI / 20));
        // 2. 方波：低 -1、高 1、周期 10 点、占空比 0.3
        slave.Value(tag2, trigger, Simulation.SquareWaveValue(-1, 1, 10, 0.3));
        // 3. 三角波：-1 ~ 1，周期 20 点
        slave.Value(tag3, trigger, Simulation.TriangleWaveValue(-1, 1, 20));
        // 4. 锯齿波：0 ~ 100，周期 20 点
        slave.Value(tag4, trigger, Simulation.SawtoothWaveValue(0, 100, 20));
        // 5. 随机值：0 ~ 100，固定种子 42 可重现
        slave.Value(tag5, trigger, Simulation.RandomValue(0, 100, seed: 42));
        // 6. 阶梯：从 0 开始，每步 +1，超过 50 回绕
        slave.Value(tag6, trigger, Simulation.StepValue(0, 1, 50));
        // 7. 常量：固定 3.14
        slave.Value(tag7, trigger, Simulation.ConstantValue(3.14));
        // 8. 带噪声的正弦波：振幅 1、偏置 0、周期 20、噪声 0.2
        slave.Value(tag8, trigger, Simulation.NoisySineWaveValue(1, 0, 20, 0.2));


        // 网关默认数据
        var port = await gateway.AddPortAsync(Ethernet.Tcp(transportDescriptor.IPEndPoint), cancellationToken);
        var dvice = await port.AddDeviceAsync(DeviceDescriptor.Create(Modbus.Ip), cancellationToken);
        await dvice.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag1, trigger), cancellationToken);
        await dvice.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag2, trigger), cancellationToken);
        await dvice.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag3, trigger), cancellationToken);
        await dvice.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag4, trigger), cancellationToken);
        await dvice.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag5, trigger), cancellationToken);
        await dvice.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag6, trigger), cancellationToken);
        await dvice.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag7, trigger), cancellationToken);
        await dvice.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag8, trigger), cancellationToken);


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