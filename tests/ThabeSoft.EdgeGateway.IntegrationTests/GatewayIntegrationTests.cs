using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using System.Collections.Concurrent;
using System.Net;
using ThabeSoft.Codecs;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.Transports.Ethernet;
using ThabeSoft.Transports.Ethernet.DependencyInjection;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.Lifecycles;
using ThabeSoft.EdgeGateway.Modbus;
using ThabeSoft.EdgeGateway.Modbus.DependencyInjection;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Simulation;
using ThabeSoft.EdgeGateway.Simulation.DependencyInjection;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.Triggers;

namespace ThabeSoft.EdgeGateway;


[TestClass]
public class GatewayIntegrationTests
{
    private IHost? _app;

    protected FakeTimeProvider TimeProvider { get; } = new();
    protected IGateway Gateway { get; private set; } = null!;
    protected ISimulator Simulator { get; private set; } = null!;


    private readonly IpTransportDescriptor _transportDescriptor = EthernetBuilder.Tcp(IPAddress.Loopback, 2334);
    private readonly ProtocolDescriptor _protocolDescriptor = ModbusBuilder.Ip;
    private readonly DataAddressDescriptor<double> _tag = ModbusBuilder.HoldingRegister(1, 0, CodecBuilder.DoubleBigEndian);
    private readonly TriggerDescriptor _trigger = TriggerBuilder.Interval(500);


    [TestInitialize]
    public async Task Init()
    {
        _app = Host.CreateDefaultBuilder()
            .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.Trace))
            .UseEdgeGateway()
            .ConfigureServices(x => x

                // 模拟器
                .AddEdgeGatewaySimulation()
                .AddEdgeGatewayModbusSimulation()

                // 网关
                .AddEdgeGatewayModbus()
                .AddEdgeGatewayEthernet())
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
        var port = Simulator.AddPort(SimulationBuilder.ModbusIpPort(_transportDescriptor));
        var device = await port.AddDeviceAsync(SimulationBuilder.ModbusDevice(1), TestContext.CancellationToken);
        await device.ValueAsync(_tag, _trigger, SimulationBuilder.SineWaveValue(1, 0.1), TestContext.CancellationToken);

        // 模拟器 启动
        await port.StartAsync(TestContext.CancellationToken);

        Assert.AreEqual(LifecycleState.Running, device.State);
        Assert.AreEqual(LifecycleState.Running, port.State);
    }


    [TestMethod]
    public async Task StartGateway()
    {
        //await StartSimulator();
        //await Task.Delay(1000);

        ConcurrentQueue<double> _values = [];

        var port = await Gateway.AddPortAsync(_transportDescriptor, TestContext.CancellationToken);
        var device = await port.AddDeviceAsync(_protocolDescriptor, TestContext.CancellationToken);
        var subscription = await device.AddSubscriptionAsync(_tag, _trigger, TestContext.CancellationToken);
        subscription.Subscribe(x =>
        {
            _values.Enqueue(x.Value);
            TestContext.WriteLine(x.ToString());
        });

        await Gateway.StartAsync(TestContext.CancellationToken);
        await Task.Delay(TimeSpan.FromMinutes(3), TestContext.CancellationToken);

        Assert.IsGreaterThan(1, _values.Count);
    }

    public TestContext TestContext { get; set; }
}