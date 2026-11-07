using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Ports;


[TestClass]
public class SerialPortTests
{
    [TestMethod]
    public async Task Gateway_Should_Read_Continuous_Data_From_TcpSimulator()
    {
        var app = Host.CreateDefaultBuilder()
            .ConfigureServices(services => services
                .AddEdgeGateway()
                .ConfigureGateway(x => x
                    .AddSerialPort("COM2")
                        .AddModbusRtuDevice()
                            .AddSubscription(Modbus.HoldingRegister(1, 0, Codec.Int16), Trigger.Interval(100))
                                .Subscribe(x => TestContext.WriteLine(x.ToString()))
                )
                .AddModbus()
                .AddSerial()
                .AddEthernet()
            )
            .Build();

        await app.StartAsync(TestContext.CancellationToken);
        await Task.Delay(10000, TestContext.CancellationToken);
    }

    public TestContext TestContext { get; set; }
}
