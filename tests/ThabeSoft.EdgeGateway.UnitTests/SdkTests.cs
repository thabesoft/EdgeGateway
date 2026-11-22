using Moq;
using System.Net;
using ThabeSoft.Codecs;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Transports.Serial;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Transports;
using ThabeSoft.Triggers;
using ThabeSoft.Protocols.Modbus;

namespace ThabeSoft.EdgeGateway.UnitTests;


[TestClass]
public class SdkTests
{
    [TestMethod]
    public async Task Gateway_AddPort()
    {
        var mockGateway = new Mock<IGateway>();
        var gateway = mockGateway.Object;



<<<<<<< TODO: 项目“ThabeSoft.EdgeGateway.UnitTests (net11.0)”的未合并的更改, 在此之前:
        await gateway.AddPortAsync(Transports.Ethernet.Transport.Tcp(IPAddress.Loopback, 8086), TestContext.CancellationToken);
        await gateway.AddPortAsync(Transports.Ethernet.Transport.Tcp(IPAddress.Loopback, 8086, new(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30)) { BackoffMultiplier = 1.5 }), TestContext.CancellationToken);
=======
        await gateway.AddPortAsync(Transports.Transport.Tcp(IPAddress.Loopback, 8086), TestContext.CancellationToken);
        await gateway.AddPortAsync(Transports.Transport.Tcp(IPAddress.Loopback, 8086, new(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30)) { BackoffMultiplier = 1.5 }), TestContext.CancellationToken);
>>>>>>> 晚于
        await gateway.AddPortAsync(Transport.Tcp(IPAddress.Loopback, 8086), TestContext.CancellationToken);
        await gateway.AddPortAsync(Transport.Tcp(IPAddress.Loopback, 8086, new(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30)) { BackoffMultiplier = 1.5 }), TestContext.CancellationToken);


        await gateway.AddPortAsync(Transports.Transport.Serial("Com1"), TestContext.CancellationToken);
        await gateway.AddPortAsync(Transports.Transport.Serial("Com1", 9600), TestContext.CancellationToken);
        await gateway.AddPortAsync(Transports.Transport.Serial("Com1", 9600, SerialParity.None), TestContext.CancellationToken);
        await gateway.AddPortAsync(Transports.Transport.Serial("Com1", 9600, SerialParity.None, 8), TestContext.CancellationToken);
        await gateway.AddPortAsync(Transports.Transport.Serial("Com1", 9600, SerialParity.None, 8, SerialStopBits.One), TestContext.CancellationToken);
        await gateway.AddPortAsync(Transports.Transport.Serial("Com1", 9600, SerialParity.None, 8, SerialStopBits.One, new(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30)) { BackoffMultiplier = 1.5 }), TestContext.CancellationToken);
        await gateway.AddPortAsync(Transports.Transport.Serial("Com1", retry: RetryBuilder.From(3000, 30000, 1.5)), TestContext.CancellationToken);
        await gateway.AddPortAsync(Transports.Transport.Serial("Com1", retry: RetryBuilder.From(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), 1.5)), TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task Port_AddDeviceAsync()
    {
        var mockPort = new Mock<IPort>();
        var port = mockPort.Object;


        await port.AddDeviceAsync(ThabeSoft.Protocols.Modbus.Modbus.Ip, TestContext.CancellationToken);
        await port.AddDeviceAsync(DeviceId.Create(), ThabeSoft.Protocols.Modbus.Modbus.Ip, TestContext.CancellationToken);
        await port.AddDeviceAsync(new() { Id = DeviceId.Create(), Protocol = ThabeSoft.Protocols.Modbus.Modbus.Ip }, TestContext.CancellationToken);


        await port.AddDeviceAsync(ThabeSoft.Protocols.Modbus.Modbus.Ascii, TestContext.CancellationToken);
        await port.AddDeviceAsync(DeviceId.Create(), ThabeSoft.Protocols.Modbus.Modbus.Ascii, TestContext.CancellationToken);
        await port.AddDeviceAsync(new() { Id = DeviceId.Create(), Protocol = ThabeSoft.Protocols.Modbus.Modbus.Ascii }, TestContext.CancellationToken);


        await port.AddDeviceAsync(ThabeSoft.Protocols.Modbus.Modbus.Rtu, TestContext.CancellationToken);
        await port.AddDeviceAsync(DeviceId.Create(), ThabeSoft.Protocols.Modbus.Modbus.Rtu, TestContext.CancellationToken);
        await port.AddDeviceAsync(new() { Id = DeviceId.Create(), Protocol = ThabeSoft.Protocols.Modbus.Modbus.Rtu }, TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task Device_AddDeviceAsync()
    {
        var mockDevice = new Mock<IDevice>();
        var device = mockDevice.Object;

        await device.AddSubscriptionAsync(ThabeSoft.Protocols.Modbus.Modbus.HoldingRegister(1, 0, ThabeSoft.Codecs.Codec.DoubleBigEndian), Trigger.Interval(100), TestContext.CancellationToken);
        await device.AddSubscriptionAsync(SubscriptionId.Create(), ThabeSoft.Protocols.Modbus.Modbus.HoldingRegister(1, 0, ThabeSoft.Codecs.Codec.DoubleBigEndian), Trigger.Interval(100), TestContext.CancellationToken);
        await device.AddSubscriptionAsync(new SubscriptionDescriptor<double>() { Id = SubscriptionId.Create(), Tag = ThabeSoft.Protocols.Modbus.Modbus.HoldingRegister(1, 0, ThabeSoft.Codecs.Codec.DoubleBigEndian), Trigger = Trigger.Interval(100) }, TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task Subscription_Subscribe()
    {
        var mockSubscription = new Mock<ISubscription<double>>();
        var subscription = mockSubscription.Object;

        using var _ = subscription.Subscribe(x => { });
    }

    public TestContext TestContext { get; set; }
}
