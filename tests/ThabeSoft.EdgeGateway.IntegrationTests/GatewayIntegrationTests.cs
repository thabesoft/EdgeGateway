using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using System.Collections.Concurrent;
using System.Net;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway;


[TestClass]
public class GatewayIntegrationTests
{
    private IHost? _app;

    protected FakeTimeProvider TimeProvider { get; } = new();
    protected IGateway Gateway { get; private set; } = null!;
    protected ISimulator Simulator { get; private set; } = null!;


    private readonly IpTransportDescriptor _transportDescriptor = Ethernet.Tcp(IPAddress.Loopback, 2333);
    private readonly IProtocolDescriptor _protocolDescriptor = Modbus.Ip;
    private readonly TagDescriptor<double> _tag = Modbus.HoldingRegister(1, 0, Codec.Double);
    private readonly ITriggerDescriptor _trigger = Trigger.Interval(500);


    [TestInitialize]
    public async Task Init()
    {
        _app = Host.CreateDefaultBuilder()
            .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.Trace))
            .ConfigureServices(x => x

                // 模拟器
                .AddSimulation()
                .AddModbusSimulation()

                // 网关
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
    public async Task StartSimulator()
    {
        var port = Simulator.AddPort(Simulation.ModbusIpPort(_transportDescriptor));
        var device = await port.AddDeviceAsync(Simulation.ModbusDevice(1), TestContext.CancellationToken);
        await device.ValueAsync(_tag, _trigger, Simulation.SineWaveValue(1, 0.1));

        // 模拟器 启动
        await port.StartAsync(TestContext.CancellationToken);

        Assert.AreEqual(LifecycleState.Running, device.State);
        Assert.AreEqual(LifecycleState.Running, port.State);
    }


    [TestMethod]
    public async Task StartGateway()
    {
        await StartSimulator();
        ConcurrentQueue<double> _values = [];

        var port = await Gateway.AddPortAsync(new(_transportDescriptor), TestContext.CancellationToken);
        var device = await port.AddDeviceAsync(_protocolDescriptor, TestContext.CancellationToken);
        var subscription = await device.AddSubscriptionAsync(_tag, _trigger, TestContext.CancellationToken);
        subscription.Subscribe(x =>
            _values.Enqueue(x.Value));

        await Gateway.StartAsync(TestContext.CancellationToken);
        await Task.Delay(1000);

        Assert.IsGreaterThan(1, _values.Count);
    }

    public TestContext TestContext { get; set; }
}