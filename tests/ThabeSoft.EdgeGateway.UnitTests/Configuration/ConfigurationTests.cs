using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using System.Reactive.Linq;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Transport.Byte;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Configuration;


[TestClass]
public class ConfigurationTests
{
    [TestMethod]
    public async Task SignalTrigger_ManualSignal_PublishesReading()
    {
        var mockTransportFactory = new Mock<ITransportConnectionFactory>();
        mockTransportFactory.Setup(x => x.CanCreate(It.IsAny<ITransportDescriptor>()))
            .Returns(true);
        mockTransportFactory.Setup(x => x.Create(It.IsAny<ITransportDescriptor>()))
            .Returns((ITransportDescriptor descriptor) =>
            {
                var mock = new Mock<ITransportConnection>();
                mock.Setup(x => x.Endpoint).Returns(descriptor.Endpoint);

                return mock.Object;
            });

        var mockProtocolAccessorFactory = new Mock<IProtocolAccessorFactory>();
        mockProtocolAccessorFactory.Setup(x => x.CanCreate(It.IsAny<IProtocolDescriptor>()))
            .Returns(true);
        mockProtocolAccessorFactory.Setup(x => x.Create(It.IsAny<IProtocolDescriptor>(), It.IsAny<IByteDuplex>()))
            .Returns((IProtocolDescriptor descriptor, IByteDuplex byteDuplex) =>
            {
                var mock = new Mock<IProtocolAccessor>();
                return mock.Object;
            });

        var received = new TaskCompletionSource();
        var trigger = Trigger.Signal(out var signal);
        int signalCount = 0;

        var deviceId = DeviceId.Create();

        using var app = Host.CreateDefaultBuilder()
            .ConfigureLogging((_, x) => x.SetMinimumLevel(LogLevel.Trace))
            .ConfigureServices(x => x
                .AddEdgeGateway(gateway => gateway
                    .AddSerialPort("COM2")
                        .AddModbusRtuDevice(deviceId)
                            .AddSubscription(Modbus.Coil(1, 0), trigger)
                                .Subscribe(_ =>
                                {
                                    signalCount++;
                                    received.TrySetResult();
                                })
                )
                .AddModbus()
                .AddSerial()
                .AddEthernet()
                .RemoveAll<IProtocolAccessorFactory>().AddSingleton(mockProtocolAccessorFactory.Object)
                .RemoveAll<ITransportConnectionFactory>().AddSingleton(mockTransportFactory.Object)
            )
            .Build();

        var gateway = app.Services.GetRequiredService<IGateway>();
        await app.StartAsync(TestContext.CancellationToken);

        Assert.HasCount(1, gateway.Ports);
        var port = gateway.Ports[ConnectionEndpoint.Serial("Com2")];
        Assert.HasCount(1, port.Devices);

        Assert.HasCount(1, port.Devices);
        var device = port.Devices[deviceId];
        Assert.HasCount(1, device.Subscriptions);

        signal.OnSignal();
        received.Task.Wait();

        Assert.AreEqual(1, signalCount);
    }


    public TestContext TestContext { get; set; }
}