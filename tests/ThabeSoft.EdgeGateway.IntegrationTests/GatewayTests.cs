using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.Simulator.ModbusSlave;

namespace ThabeSoft.EdgeGateway;


[TestClass]
public class GatewayTests
{
    [TestMethod]
    public async Task Fc()
    {
        using var app = Host.CreateDefaultBuilder()
            .ConfigureServices(x => x
                .AddModbusRtuSimulator(x =>
                {
                    x.UnitId = 1;
                    x.SerialPortOptions = new SerialPortOptions() { PortName = "COM3" };
                })
                .AddEdgeGateway()
                .AddModbus()
                .AddSerial()
                //.RemoveAll<ITransportFactory>()
                //.AddSingleton<ITransportFactory, MockTransportFactory>()
                )
            .Build();

        var serialSimulator = app.Services.GetRequiredService<IModbusSerialSimulator>();
        serialSimulator.SetHoldingRegister(0, 123);


        var gateway = app.Services.GetRequiredService<IGateway>();


        var port = await gateway.AddPortAsync(Serial.Port("COM2"));
        var device = await port.AddDeviceAsync(Modbus.Rtu);

        var tag = Tags.Modbus.HoldingRegister(1, 0, WordCodec.BigEndian.AsShort());
        var subscription = await device.AddSubscriptionAsync(tag, Trigger.Interval(10));

        using var _ = subscription.Tag.Subscribe(x =>
            TestContext.WriteLine(x.ToString()));

        await app.StartAsync(TestContext.CancellationToken);
        await Task.Delay(100, TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task FromBuilder()
    {
        using var app = Host.CreateDefaultBuilder()
            .ConfigureServices(x => x
                .AddModbusRtuSimulator(x =>
                {
                    x.UnitId = 1;
                    x.SerialPortOptions = new SerialPortOptions() { PortName = "COM3" };
                })
                .AddEdgeGateway(gateway =>
                {
                    gateway.AddSerialPort("COM2", configure: port =>
                    {
                        port.AddModbusRtuDevice(x =>
                        {
                            x.AddSubscription(Modbus.HoldingRegister(0, 0, Codec.Int16), Trigger.Interval(10),
                                x => TestContext.WriteLine(x.ToString()));
                        });

                    });
                })
                .AddModbus()
                .AddSerial()
                )
            .Build();

        var serialSimulator = app.Services.GetRequiredService<IModbusSerialSimulator>();
        serialSimulator.SetHoldingRegister(0, 123);
        serialSimulator.SetHoldingRegister(10, 456);


        var gateway = app.Services.GetRequiredService<IGateway>();
        await app.StartAsync(TestContext.CancellationToken);

        await Task.Delay(100, TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task FromBuilderV2()
    {
        void ConfigAction(IGatewayOptionsConfiguratorV2 gateway)
        {
            gateway
                // 端口1
                .AddPort(Serial.Port("COM3"))
                   // 设备1
                   .AddDevice(DeviceId.Create(), Modbus.Ascii)
                       // 温度
                       .AddSubscription(Modbus.Coil(1, 1), Trigger.Interval(100))
                           .OnValue(x => TestContext.WriteLine(x.ToString()))
                       // 水位
                       .AddSubscription(Modbus.HoldingRegister(0, 0, Codec.UInt16), Trigger.Interval(10))
                           .OnValue(x => TestContext.WriteLine(x.ToString()))

                   // 设备2
                   .AddDevice(DeviceId.Create(), Modbus.Rtu)
                       // 速度
                       .AddSubscription(Modbus.InputRegister(1, 100, Codec.Int16), Trigger.Interval(10))
                           .OnValue(x => TestContext.WriteLine(x.ToString()))
                // 端口 2
                .AddPort(Ethernet.Tcp(IPAddress.Loopback, 80));
        }



    }



    public TestContext TestContext { get; set; }
}



file sealed class MockTransportFactory(ILoggerFactory loggerFactory) : ITransportFactory
{
    public bool CanCreate(ITransportDescriptor descriptor) => true;
    public ITransportConnection Create(ITransportDescriptor descriptor) =>
        new MockTransportConnection(descriptor, loggerFactory.CreateLogger<MockTransportConnection>());


    private sealed class MockTransportConnection(ITransportDescriptor transportDescriptor, ILogger<MockTransportConnection> logger) : LifecycleStateMachine, ITransportConnection
    {
        public ConnectionEndpoint Endpoint => transportDescriptor.Endpoint;

        public ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("读取数据");
            return ValueTask.FromResult(0);
        }
        public ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("写入数据, 长度:{Len}", data.Length);
            return ValueTask.CompletedTask;
        }

        protected override ValueTask OnStartAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("已启动");
            return ValueTask.CompletedTask;
        }
        protected override ValueTask OnStopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("已停止");
            return ValueTask.CompletedTask;
        }
    }
}

