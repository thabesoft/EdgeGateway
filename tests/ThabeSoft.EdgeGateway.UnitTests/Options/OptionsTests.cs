using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Options;


[TestClass]
public class OptionsTests
{
    [TestMethod]
    public void TestMethod1()
    {
        ServiceCollection services = new();
        services.AddGatewayOptions(gateway => gateway
            .AddSerialPort("Com2", portBuildAction: port => port
                .AddModbusRtuDevice(device => device
                    .AddTag(tag => tag
                        .WithName("测试")
                        .WithFixedIntervalTrigger(1000)
                        .WithDescriptor(ModbusTag.Coil(1, 100))))));


    }
}
