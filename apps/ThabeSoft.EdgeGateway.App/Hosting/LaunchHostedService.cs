using Microsoft.Extensions.Hosting;
using System.Net;
using ThabeSoft.EdgeGateway.App.Features.Gateway;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport;
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
        await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2333), simulator, gateway, cancellationToken);
        await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2334), simulator, gateway, cancellationToken);
        await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2335), simulator, gateway, cancellationToken);
        await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2336), simulator, gateway, cancellationToken);
        await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2337), simulator, gateway, cancellationToken);


        await Task.Delay(10, cancellationToken);
        await simulator.StartAsync(cancellationToken);
        await Task.Delay(2000, cancellationToken);
        await gateway.StartCommand.ExecuteAsync(null);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async Task AddPortAsync(IpTransportDescriptor transportDescriptor, ISimulator simulator, GatewayViewModel gateway, CancellationToken cancellationToken)
    {
        var network = simulator.AddPort(Simulation.ModbusIpPort(transportDescriptor));
        var port = await gateway.AddPortAsync(Ethernet.Tcp(transportDescriptor.IPEndPoint), cancellationToken);

        const int count = 10;

        for (byte i = 1; i <= count; i++)
            await AddDeviceAsync(i, network, port, cancellationToken);
    }
    private static async Task AddDeviceAsync(byte id, ISimulationPort network, PortViewModel port, CancellationToken cancellationToken)
    {
        var slave = await network.AddDeviceAsync(Simulation.ModbusDevice(id), cancellationToken);
        var device = await port.AddDeviceAsync(DeviceDescriptor.Create(Modbus.Ip), cancellationToken);

        var trigger = Trigger.Interval(100);   // 50 个标签，100ms 足够，别用 10ms

        const int count = 1000;
        var generators = CreateGenerators(count);

        for (int i = 0; i < count; i++)
        {
            // Double 占 4 个寄存器，地址按 4 递增
            var tag = Modbus.HoldingRegister(id, (ushort)(i * 4), Codec.Double);

            // 模拟从站写入
            await slave.ValueAsync(tag, trigger, generators[i], cancellationToken);
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