using ThabeSoft.Codecs;
using ThabeSoft.Configuration.Diff;
using ThabeSoft.Configuration.Models;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Modbus;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Serial;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Triggers;

namespace ThabeSoft.EdgeGateway.IntegrationTests;

[TestClass]
public class ConfigDiffCalculatorTests
{
    [TestMethod]
    public void TestMethod1()
    {
        var old = new GatewayConfig()
        {
            Ports =
            [
                new()
                {
                    Descriptor = new PortDescriptor()
                    {
                        Id = PortId.From("PortId"),
                        Transport = new SerialTransportDescriptor()
                        {
                             PortName = "Com1",
                             BaudRate = 9600,
                             Retry = new Transports.RetryDescriptor(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30))
                             {
                                  BackoffMultiplier = 1.5
                             }
                        }
                    },
                    Devices =
                    [
                        new DeviceConfig()
                        {
                             Descriptor =new()
                             {
                                 Id = DeviceId.From("DeviceId"),
                                 Protocol = ModbusBuilder.Rtu
                             },
                             Subscriptions  =
                             [
                                 new SubscriptionConfig()
                                 {
                                      Descriptor = new SubscriptionDescriptor<int>()
                                      {
                                          Id = SubscriptionId.From("SubscriptionId"),
                                          Tag = ModbusBuilder.HoldingRegister(1, 0, CodecBuilder.Int32Abcd),
                                          Trigger = TriggerBuilder.Interval(100)
                                      }
                                 }
                             ]
                        }
                    ]
                }
            ]
        };

        var calc = new ConfigDiffCalculator();
        calc.Diff();
    }
}
