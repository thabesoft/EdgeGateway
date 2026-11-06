using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using System.Net;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Configuration;


[TestClass]
public class ConfigurationTests
{
    [TestMethod]
    public async Task TestMethod1()
    {
        var mockBoolSubscription = new Mock<ISubscription<bool>>();
        var mockFloatSubscription = new Mock<ISubscription<float>>();

        var mockDevice = new Mock<IDevice>();
        mockDevice.Setup(x => x.AddSubscriptionAsync(It.IsAny<SubscriptionDescriptor<bool>>()))
            .ReturnsAsync(mockBoolSubscription.Object);
        mockDevice.Setup(x => x.AddSubscriptionAsync(It.IsAny<SubscriptionDescriptor<float>>()))
            .ReturnsAsync(mockFloatSubscription.Object);


        var mockPort = new Mock<IPort>();
        mockPort.Setup(x => x.AddDeviceAsync(It.IsAny<DeviceDescriptor>()))
            .ReturnsAsync(mockDevice.Object);

        var mockGateway = new Mock<IGateway>();
        mockGateway.Setup(x => x.AddPortAsync(It.IsAny<ITransportDescriptor>()))
            .ReturnsAsync(mockPort.Object);

        using var app = Host.CreateDefaultBuilder()
            .ConfigureServices(x => x
                .AddEdgeGateway(gateway => gateway
                    .AddSerialPort("COM2")
                        .AddModbusRtuDevice()
                            .AddSubscription(Modbus.Coil(1, 0), Trigger.Interval(10))
                            .OnValue(x => { })
                    .AddTcpPort(IPAddress.Loopback, 8086)
                        .AddModbusIpDevice()
                            .AddSubscription(Modbus.HoldingRegister(1, 0, Codec.Float), Trigger.Interval(10))
                )
                .AddModbus()
                .AddSerial()
                .AddSingleton<IGateway>(mockGateway.Object)
            )
            .Build();

        var gateway = app.Services.GetRequiredService<IGateway>();
        await app.StartAsync(TestContext.CancellationToken);
        await Task.Delay(100, TestContext.CancellationToken);


        mockGateway.Verify(x => x.AddPortAsync(It.IsAny<ITransportDescriptor>()), Times.Exactly(2));

        mockDevice.Verify(x => x.AddSubscriptionAsync(It.IsAny<SubscriptionDescriptor<bool>>()), Times.Once);
        mockDevice.Verify(x => x.AddSubscriptionAsync(It.IsAny<SubscriptionDescriptor<ushort>>()), Times.Once);

        mockBoolSubscription.Verify(x => x.Tag.Subscribe(It.IsAny<Action<bool>>()), Times.Once());
    }

    public TestContext TestContext { get; set; }
}
