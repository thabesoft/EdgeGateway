using Microsoft.Extensions.Hosting;
using System.Net;
using ThabeSoft.EdgeGateway.App.Features.Gateway;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.ValueGenerators;

namespace ThabeSoft.EdgeGateway.App.Hosting;


internal sealed class LaunchHostedService(
    ISimulator simulator,
    GatewayViewModel gateway
    ) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var transportDescriptor = Ethernet.Tcp(IPAddress.Loopback, 2333);

        // 从站模拟
        var network = simulator.AddPort(Simulation.ModbusIpPort(transportDescriptor));
        var slave1 = await network.AddDeviceAsync(Simulation.ModbusDevice(1), cancellationToken);
        var slave2 = await network.AddDeviceAsync(Simulation.ModbusDevice(2), cancellationToken);
       
        // 网关默认数据
        var port = await gateway.AddPortAsync(Ethernet.Tcp(transportDescriptor.IPEndPoint), cancellationToken);
        var device1 = await port.AddDeviceAsync(DeviceDescriptor.Create(Modbus.Ip), cancellationToken);
        var device2 = await port.AddDeviceAsync(DeviceDescriptor.Create(Modbus.Ip), cancellationToken);


        // 初始化数据
        await AddTagAsync(1, slave1, device1, cancellationToken);
        await AddTagAsync(2, slave2, device2, cancellationToken);


        await Task.Delay(10, cancellationToken);
        await simulator.StartAsync(cancellationToken);
        await Task.Delay(2000, cancellationToken);
        await gateway.StartCommand.ExecuteAsync(null);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async Task AddTagAsync(byte id, ISimulationDevice slave, DeviceViewModel device, CancellationToken cancellationToken)
    {
        var trigger = Trigger.Interval(100);   // 50 个标签，100ms 足够，别用 10ms

        const int count = 1;
        var generators = CreateGenerators(count);

        for (int i = 0; i < count; i++)
        {
            // Double 占 4 个寄存器，地址按 4 递增
            var tag = Modbus.HoldingRegister(id, (ushort)(i * 4), Codec.Double);

            // 模拟从站写入
            slave.Value(tag, trigger, generators[i]);

            // 网关订阅
            await device.AddSubscriptionAsync(SubscriptionDescriptor.Create(tag, trigger), cancellationToken);
        }
    }

    private static IValueGenerator<double>[] CreateGenerators(int count)
    {
        var generators = new IValueGenerator<double>[count];
        for (int i = 0; i < count; i++)
        {
            // 8 种生成器循环复用
            generators[i] = (i % 8) switch
            {
                0 => Simulation.SineWaveValue(1, 2 * Math.PI / 20),
                1 => Simulation.SquareWaveValue(-1, 1, 10, 0.3),
                2 => Simulation.TriangleWaveValue(-1, 1, 20),
                3 => Simulation.SawtoothWaveValue(0, 100, 20),
                4 => Simulation.RandomValue(0, 100, seed: 42 + i),   // 每个标签不同种子，避免完全同步
                5 => Simulation.StepValue(0, 1, 50),
                6 => Simulation.ConstantValue(3.14),
                7 => Simulation.NoisySineWaveValue(1, 0, 20, 0.2),
                _ => throw new InvalidOperationException()
            };
        }
        return generators;
    }
}