using Microsoft.Extensions.DependencyInjection;

namespace ThabeSoft.EdgeGateway.IntegrationTests.Ports;


[TestClass]
public class AddSerialPortTests : GatewayTestBase
{
    protected override void OnConfigureServices(ServiceCollection descriptors)
    {
        descriptors.AddSerial();
    }


    [TestMethod]
    public void TestMethod1()
    {
        var port = Gateway.AddSerialPort(x => x
            .PortName("COM2")
            .BaudRate(9600)
            .Parity(System.IO.Ports.Parity.None)
            .StopBits(System.IO.Ports.StopBits.One)
            .DataBits(8));
    }
}
