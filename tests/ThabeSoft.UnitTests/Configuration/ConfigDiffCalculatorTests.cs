using ThabeSoft.Codecs;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Configuration.Diff;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Protocols.Modbus;
using ThabeSoft.Transports;
using ThabeSoft.Transports.Serial;
using ThabeSoft.Triggers;

namespace ThabeSoft.UnitTests.Configuration;


[TestClass]
public class ConfigDiffCalculatorTests
{
    [TestMethod]
    public void Diff_DoesNotThrow()
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
                            Retry = new RetryDescriptor(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30))
                            {
                                BackoffMultiplier = 1.5
                            }
                        }
                    },
                    Devices =
                    [
                        new DeviceConfig()
                        {
                            Descriptor = new()
                            {
                                Id = DeviceId.From("DeviceId"),
                                Protocol = Modbus.Rtu
                            },
                            Subscriptions =
                            [
                                new SubscriptionConfig()
                                {
                                    Descriptor = new SubscriptionDescriptor<int>()
                                    {
                                        Id = SubscriptionId.From("SubscriptionId"),
                                        Tag = Modbus.HoldingRegister(1, 0, Codec.Int32Abcd),
                                        Trigger = Trigger.Interval(100)
                                    }
                                }
                            ]
                        }
                    ]
                }
            ]
        };

        var @new = new GatewayConfig();

        var calculator = new ConfigDiffCalculator();
        calculator.Diff(old, @new);
        calculator.Diff(@new, old);
        calculator.Diff(old, old);
    }
}
