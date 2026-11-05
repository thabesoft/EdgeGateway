using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.Simulator.ModbusSlave;

namespace ThabeSoft.EdgeGateway.Configuration;


[TestClass]
public class OptionsTests
{
    [TestMethod]
    public async Task TestMethod1()
    {
        using var host = Host.CreateDefaultBuilder()
            .ConfigureServices(x =>
            {
                x.AddModbusRtuSimulator(x =>
                {
                    x.UnitId = 1;
                    x.SerialPortOptions = new() { PortName = "COM3" };
                });

                x.AddSerial();
                x.AddModbus();

                x.AddEdgeGateway(gateway =>
                {
                    gateway.AddSerialPort("Com2", configure: port =>
                    {
                        port.AddModbusRtuDevice(device =>
                        {
                            device.AddTagWithFixedInterval(ModbusTags.HoldingRegister(1, 100, WordCodec.BigEndian), 100);
                            device.AddTagWithFixedInterval(ModbusTags.HoldingRegister(1, 100, WordCodec.BigEndian), 100);
                            device.AddTagWithFixedInterval(ModbusTags.HoldingRegister(1, 100, WordCodec.BigEndian), 100);
                        });
                    });
                });
            })
            .Build();

        var simulator = host.Services.GetRequiredService<IModbusSimulator>();
        simulator.SetHoldingRegister(100, 100);
        simulator.SetHoldingRegister(102, 102);
        simulator.SetHoldingRegister(104, 104);


        await host.StartAsync(TestContext.CancellationToken);


        await using var gateway = host.Services.GetRequiredService<IGateway>();

        await Task.Delay(3000, TestContext.CancellationToken);

        Assert.HasCount(1, gateway.Ports);
    }

    public TestContext TestContext { get; set; }
}
