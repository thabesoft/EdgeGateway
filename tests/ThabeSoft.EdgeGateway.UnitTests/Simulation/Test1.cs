using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;




<<<<<<< TODO: 项目“ThabeSoft.EdgeGateway.UnitTests (net11.0)”的未合并的更改, 在此之前:
=======
using ThabeSoft.EdgeGateway.Ethernet;
using ThabeSoft.EdgeGateway.Modbus;
>>>>>>> 晚于
<<<<<<< TODO: 项目“ThabeSoft.EdgeGateway.UnitTests (net11.0)”的未合并的更改, 在此之前:
using ThabeSoft.EdgeGateway.Ethernet.Ethernet;
using ThabeSoft.EdgeGateway.Ethernet.Ethernet.Ethernet;

=======
using ThabeSoft.EdgeGateway.Ethernet;
using ThabeSoft.EdgeGateway.Ethernet.Ethernet;
using ThabeSoft.EdgeGateway.Modbus;
>>>>>>> 晚于
<<<<<<< TODO: 项目“ThabeSoft.EdgeGateway.UnitTests (net11.0)”的未合并的更改, 在此之前:
=======
using ThabeSoft.EdgeGateway.Modbus;
using ThabeSoft.EdgeGateway.Modbus.Modbus;
using ThabeSoft.EdgeGateway.Simulation;
>>>>>>> 晚于
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.ValueGenerators;

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
                .AddEdgeGateway()
                .AddModbus()
                .AddSerial()
                .AddEthernet()
                )
            .Build();

        var simulatorHost = app.Services.GetRequiredService<ISimulator>();
        var gateway = app.Services.GetRequiredService<IGateway>();

        var network = simulatorHost.AddPort(Modbus.IpSimulationNetwork(address));
        var slave = await network.AddSlaveAsync(1);
        slave.Value(tag, trigger, ValueGenerators.Simulation.SineWave(1, 0.1));

        await app.StartAsync(TestContext.CancellationToken);

        await Task.Delay(1000);

        await gateway.StartAsync();
        await Task.Delay(2000, TestContext.CancellationToken);
    }

    public TestContext TestContext { get; set; }
}
