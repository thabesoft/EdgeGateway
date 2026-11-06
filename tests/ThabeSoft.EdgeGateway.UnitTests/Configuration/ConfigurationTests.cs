using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Reactive.Linq;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Configuration;


[TestClass]
public class ConfigurationTests
{
    [TestMethod]
    public async Task TestMethod1()
    {
        using var app = Host.CreateDefaultBuilder()
            .ConfigureLogging((_, x) => x.SetMinimumLevel(LogLevel.Trace))
            .ConfigureServices(x => x
                .AddEdgeGateway(gateway => gateway
                    .AddSerialPort("COM2")
                        .AddModbusRtuDevice()
                            .AddSubscription(Modbus.Coil(1, 0), Trigger.Interval(10))
                                .Observable(x => x.Where(x => x.Quality == ReadingQuality.Good).Subscribe())
                    .AddTcpPort(IPAddress.Loopback, 8086)
                        .AddModbusIpDevice()
                            .AddSubscription(Modbus.HoldingRegister(1, 0, Codec.Float), Trigger.Interval(10))
                )
                .AddModbus()
                .AddSerial()
            )
            .Build();

        var gateway = app.Services.GetRequiredService<IGateway>();
        await app.StartAsync(TestContext.CancellationToken);
        await Task.Delay(100, TestContext.CancellationToken);
    }


    public TestContext TestContext { get; set; }
}