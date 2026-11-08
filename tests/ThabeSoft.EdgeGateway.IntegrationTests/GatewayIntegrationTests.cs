using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Simulation;

namespace ThabeSoft.EdgeGateway;


[TestClass]
public class GatewayIntegrationTests
{
    private IHost? _app;

    protected IGateway Gateway { get; private set; } = null!;
    protected ISimulatorHost Simulator { get; private set; } = null!;


    [TestInitialize]
    public async Task Init()
    {
        _app = Host.CreateDefaultBuilder()
            .ConfigureServices(x => x
                .AddSimulation()
                .AddEdgeGateway()
                .AddModbus()
                .AddEthernet())
            .Build();

        Gateway = _app.Services.GetRequiredService<IGateway>();
        Simulator = _app.Services.GetRequiredService<ISimulatorHost>();
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

        var simulationNetwork = Simulator.AddNetwork(Modbus.IpSimulationNetwork(transportDescriptor));
        await simulationNetwork.StartAsync(TestContext.CancellationToken);

        var port = Gateway.AddPort(transportDescriptor);
        var device = port.AddDevice(protocolDescriptor);

        // 端口未启动, 启动设备则异常
        await Assert.ThrowsAsync<InvalidOperationException>(async () =>
             await device.StartAsync(TestContext.CancellationToken));
        Assert.AreEqual(LifecycleState.Faulted, device.State);

        // 启动网关, 端口设备一起启动
        await Gateway.StartAsync(TestContext.CancellationToken);
        Assert.AreEqual(LifecycleState.Running, port.State);
        Assert.AreEqual(LifecycleState.Running, device.State);

        // 端口释放, 设备同步释放
        await port.DisposeAsync();
        Assert.AreEqual(LifecycleState.Disposed, port.State);
        Assert.AreEqual(LifecycleState.Disposed, device.State);

        // 网关释放, 所有资源不可用
        await Gateway.DisposeAsync();
        Assert.AreEqual(LifecycleState.Disposed, Gateway.State);
        Assert.Throws<ObjectDisposedException>(() =>
            port.AddDevice(Modbus.Ip));
    }

    public TestContext TestContext { get; set; }
}