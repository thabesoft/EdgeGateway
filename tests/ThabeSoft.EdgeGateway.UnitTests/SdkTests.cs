using Moq;
using System.Net;
using ThabeSoft.Codecs;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.Transports.Ethernet;
using ThabeSoft.EdgeGateway.Modbus;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Transports.Serial;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.Transports;
using ThabeSoft.Triggers;

namespace ThabeSoft.EdgeGateway.UnitTests;


[TestClass]
public class SdkTests
{
    [TestMethod]
    public async Task Gateway_AddPort()
    {
        var mockGateway = new Mock<IGateway>();
        var gateway = mockGateway.Object;


        await gateway.AddPortAsync(EthernetBuilder.Tcp(IPAddress.Loopback, 8086), TestContext.CancellationToken);
        await gateway.AddPortAsync(EthernetBuilder.Tcp(IPAddress.Loopback, 8086, new(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30)) { BackoffMultiplier = 1.5 }), TestContext.CancellationToken);


        await gateway.AddPortAsync(SerialBuilder.Port("Com1"), TestContext.CancellationToken);
        await gateway.AddPortAsync(SerialBuilder.Port("Com1", 9600), TestContext.CancellationToken);
        await gateway.AddPortAsync(SerialBuilder.Port("Com1", 9600, SerialParity.None), TestContext.CancellationToken);
        await gateway.AddPortAsync(SerialBuilder.Port("Com1", 9600, SerialParity.None, 8), TestContext.CancellationToken);
        await gateway.AddPortAsync(SerialBuilder.Port("Com1", 9600, SerialParity.None, 8, SerialStopBits.One), TestContext.CancellationToken);
        await gateway.AddPortAsync(SerialBuilder.Port("Com1", 9600, SerialParity.None, 8, SerialStopBits.One, new(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30)) { BackoffMultiplier = 1.5 }), TestContext.CancellationToken);
        await gateway.AddPortAsync(SerialBuilder.Port("Com1", retry: RetryBuilder.From(3000, 30000, 1.5)), TestContext.CancellationToken);
        await gateway.AddPortAsync(SerialBuilder.Port("Com1", retry: RetryBuilder.From(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), 1.5)), TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task Port_AddDeviceAsync()
    {
        var mockPort = new Mock<IPort>();
        var port = mockPort.Object;


        await port.AddDeviceAsync(ModbusBuilder.Ip, TestContext.CancellationToken);
        await port.AddDeviceAsync(DeviceId.Create(), ModbusBuilder.Ip, TestContext.CancellationToken);
        await port.AddDeviceAsync(new() { Id = DeviceId.Create(), Protocol = ModbusBuilder.Ip }, TestContext.CancellationToken);


        await port.AddDeviceAsync(ModbusBuilder.Ascii, TestContext.CancellationToken);
        await port.AddDeviceAsync(DeviceId.Create(), ModbusBuilder.Ascii, TestContext.CancellationToken);
        await port.AddDeviceAsync(new() { Id = DeviceId.Create(), Protocol = ModbusBuilder.Ascii }, TestContext.CancellationToken);


        await port.AddDeviceAsync(ModbusBuilder.Rtu, TestContext.CancellationToken);
        await port.AddDeviceAsync(DeviceId.Create(), ModbusBuilder.Rtu, TestContext.CancellationToken);
        await port.AddDeviceAsync(new() { Id = DeviceId.Create(), Protocol = ModbusBuilder.Rtu }, TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task Device_AddDeviceAsync()
    {
        var mockDevice = new Mock<IDevice>();
        var device = mockDevice.Object;

        await device.AddSubscriptionAsync(ModbusBuilder.HoldingRegister(1, 0, CodecBuilder.DoubleBigEndian), TriggerBuilder.Interval(100), TestContext.CancellationToken);
        await device.AddSubscriptionAsync(SubscriptionId.Create(), ModbusBuilder.HoldingRegister(1, 0, CodecBuilder.DoubleBigEndian), TriggerBuilder.Interval(100), TestContext.CancellationToken);
        await device.AddSubscriptionAsync(new SubscriptionDescriptor<double>() { Id = SubscriptionId.Create(), Tag = ModbusBuilder.HoldingRegister(1, 0, CodecBuilder.DoubleBigEndian), Trigger = TriggerBuilder.Interval(100) }, TestContext.CancellationToken);
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
