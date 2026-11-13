using Microsoft.Extensions.Hosting;
using System.Net;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.ValueGenerators;

namespace ThabeSoft.EdgeGateway.Simulator;


internal sealed class LaunchHostedService(ISimulator simulator) : IHostedService
{
    private const int DeviceCount = 10;
    private const int DeviceValueCount = 1000;
    private static ITriggerDescriptor TriggerDescriptor { get; } = Trigger.Interval(10);


    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2333), simulator,cancellationToken);
        await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2334), simulator,cancellationToken);
        await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2335), simulator,cancellationToken);
        await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2336), simulator,cancellationToken);
        await AddPortAsync(Ethernet.Tcp(IPAddress.Loopback, 2337), simulator,cancellationToken);

        await simulator.StartAsync(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async Task AddPortAsync(IpTransportDescriptor transportDescriptor, ISimulator simulator, CancellationToken cancellationToken)
    {
        var network = simulator.AddPort(Simulation.ModbusIpPort(transportDescriptor));

        for (byte i = 1; i <= DeviceCount; i++)
            await AddDeviceAsync(i, network, cancellationToken);
    }
    private static async Task AddDeviceAsync(byte id, ISimulationPort network, CancellationToken cancellationToken)
    {
        var slave = await network.AddDeviceAsync(Simulation.ModbusDevice(id), cancellationToken);

        var generators = CreateGenerators(DeviceValueCount);

        for (int i = 0; i < DeviceValueCount; i++)
        {
            // Double 占 4 个寄存器，地址按 4 递增
            var tag = Modbus.HoldingRegister(id, (ushort)(i * 4), Codec.Double);
            // 模拟从站写入
            await slave.ValueAsync(tag, TriggerDescriptor, generators[i], cancellationToken);
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