using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Time.Testing;
using System.Collections.Concurrent;
using System.Net;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.ValueGenerators;

namespace ThabeSoft.EdgeGateway;


[TestClass]
public class GatewayIntegrationTests
{
    private IHost? _app;

    protected FakeTimeProvider TimeProvider { get; } = new();
    protected IGateway Gateway { get; private set; } = null!;
    protected ISimulator Simulator { get; private set; } = null!;


    [TestInitialize]
    public async Task Init()
    {
        _app = Host.CreateDefaultBuilder()
            .ConfigureServices(x => x
                .AddSingleton<TimeProvider>(TimeProvider)
                .AddSimulation()
                .AddEdgeGateway()
                .AddModbus()
                .AddEthernet())
            .Build();

        Gateway = _app.Services.GetRequiredService<IGateway>();
        Simulator = _app.Services.GetRequiredService<ISimulator>();
    }

    [TestCleanup]
    public async Task Cleanup()
    {
        _app?.Dispose();
    }


    [TestMethod]
    public async Task Lifecycle()
    {
        var transportDescriptor = Ethernet.Tcp(IPAddress.Loopback, 2333);
        var protocolDescriptor = Modbus.Ip;
        var tag = Modbus.HoldingRegister(1, 0, Codec.Double);
        var triggr = Trigger.Interval(100);

        var simulationNetwork = Simulator.AddPort(Simulation.ModbusIpPort(transportDescriptor));
        var slave = await simulationNetwork.AddDeviceAsync(Simulation.ModbusDevice(1), TestContext.CancellationToken);
        using var _ = slave.Value(tag, triggr, Simulation.SineWaveValue(1, 0.1));

        // 模拟器 启动
        await simulationNetwork.StartAsync(TestContext.CancellationToken);

        ConcurrentQueue<double> _values = [];

        var port = await Gateway.AddPortAsync(transportDescriptor, TestContext.CancellationToken);
        var device = await port.AddDeviceAsync(protocolDescriptor, TestContext.CancellationToken);
        var subscription = await device.AddSubscriptionAsync(tag, triggr, TestContext.CancellationToken);
        subscription.Subscribe(x => _values.Enqueue(x.Value));

        await Gateway.StartAsync(TestContext.CancellationToken);

        TimeProvider.Advance(TimeSpan.FromSeconds(1));
        Assert.IsGreaterThan(1, _values.Count);
    }

    public TestContext TestContext { get; set; }
}