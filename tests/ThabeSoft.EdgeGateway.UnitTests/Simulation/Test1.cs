using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Simulation;
using ThabeSoft.EdgeGateway.Simulation.ValueGenerators;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Simulator;

[TestClass]
public class Test1
{
    [TestMethod]
    public async Task TestMethod1()
    {
        var tag = Modbus.HoldingRegister(1, 0, Codec.Double);
        var trigger = Trigger.Interval(100);
        var address = Ethernet.Tcp(IPAddress.Loopback, 502);

        var app = Host.CreateDefaultBuilder()
            .ConfigureServices(x => x
                .AddSimulation()
                .AddEdgeGateway(x => x
                    .AddTcpPort(address.IPEndPoint)
                        .AddModbusIpDevice()
                            .AddSubscription(tag, trigger)
                                .Subscribe(x => TestContext.WriteLine(x.ToString()), ex => TestContext.WriteLine(ex.Message))
                )
                .AddModbus()
                .AddSerial()
                .AddEthernet()
                )
            .Build();

        var simulatorHost = app.Services.GetRequiredService<ISimulatorHost>();
        var gateway = app.Services.GetRequiredService<IGateway>();

        var network = simulatorHost.AddNetwork(Modbus.IpSimulationNetwork(address));
        var slave = network.AddSlave(1);
        slave.Value(tag, trigger, ValueGenerator.SineWave(1, 0.1));

        await app.StartAsync(TestContext.CancellationToken);

        await Task.Delay(1000);

        await gateway.StartAsync();
        await Task.Delay(2000, TestContext.CancellationToken);
    }

    public TestContext TestContext { get; set; }
}
