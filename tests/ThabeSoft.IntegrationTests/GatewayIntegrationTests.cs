using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Net;
using ThabeSoft.Codecs;
using ThabeSoft.Codecs.DependencyInjection;
using ThabeSoft.EdgeGateway.DependencyInjection;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Simulation;
using ThabeSoft.EdgeGateway.Simulation.DependencyInjection;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.DependencyInjection;
using ThabeSoft.PubSub.DependencyInjection;
using ThabeSoft.Protocols;
using ThabeSoft.Protocols.Accessor;
using ThabeSoft.Protocols.DependencyInjection;
using ThabeSoft.Protocols.Modbus;
using ThabeSoft.Protocols.Modbus.DependencyInjection;
using ThabeSoft.Transports;
using ThabeSoft.Transports.DependencyInjection;
using ThabeSoft.Transports.Ethernet;
using ThabeSoft.Transports.Ethernet.DependencyInjection;
using ThabeSoft.Triggers;
using ThabeSoft.Triggers.DependencyInjection;
using Ethernet = ThabeSoft.Transports.Ethernet.Ethernet;

namespace ThabeSoft.EdgeGateway;


[TestClass]
public class GatewayIntegrationTests
{
    private IHost? _app;

    private readonly IpTransportDescriptor _transportDescriptor = Ethernet.Tcp(IPAddress.Loopback, 2334);
    private readonly ProtocolDescriptor _protocolDescriptor = Modbus.Ip;
    private readonly DataAccessDescriptor<double> _tag = Modbus.HoldingRegister(1, 0, Codec.DoubleBigEndian);
    private readonly TriggerDescriptor _trigger = Trigger.Interval(500);


    protected IGateway Gateway { get; private set; } = null!;
    protected ISimulator Simulator { get; private set; } = null!;


    [TestInitialize]
    public async Task Init()
    {
        _app = Host.CreateDefaultBuilder()
            .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.Trace))
            .ConfigureServices(x => x

                // 网关
                .AddEdgeGateway()

                // 基础设施
                .AddLifecycles()
                .AddCodecs()
                .AddTriggers()
                .AddPubSub()

                // 模拟器
                .AddEdgeGatewaySimulation()
                .AddEdgeGatewayModbusSimulation()

                // 网关
                .AddModbusProtocol()
                .AddProtocols()
                .AddTransports()
                .AddEthernetTransport())
            .Build();

        Gateway = _app.Services.GetRequiredService<IGateway>();
        Simulator = _app.Services.GetRequiredService<ISimulator>();

        await Task.CompletedTask;
    }

    [TestCleanup]
    public void Cleanup()
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


    /// <summary>
    /// 需要外部 Modbus 从站（127.0.0.1:2334）配合，默认不参与自动化测试。
    /// </summary>
    [TestMethod]
    [Ignore("需要外部 Modbus 从站监听 127.0.0.1:2334")]
    public async Task StartGateway()
    {
        ConcurrentQueue<double> values = [];

        var port = await Gateway.AddPortAsync(_transportDescriptor, TestContext.CancellationToken);
        var device = await port.AddDeviceAsync(new DeviceDescriptor() { Protocol = _protocolDescriptor }, TestContext.CancellationToken);
        ISubscription<double> subscription = await device.AddSubscriptionAsync(_tag, _trigger, TestContext.CancellationToken);
        subscription.Subscribe(x =>
        {
            values.Enqueue(x.Value);
            TestContext.WriteLine(x.ToString());
        });

        await Gateway.StartAsync(TestContext.CancellationToken);
        await Task.Delay(TimeSpan.FromSeconds(5), TestContext.CancellationToken);

        Assert.IsGreaterThan(0, values.Count);
    }

    public TestContext TestContext { get; set; }
}
