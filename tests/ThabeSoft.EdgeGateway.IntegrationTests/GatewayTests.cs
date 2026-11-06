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

        var tag = Modbus.HoldingRegister(1, 0, WordCodec.BigEndian.AsShort());
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
                .AddEdgeGateway(gateway => gateway
                    .AddSerialPort("COM2")
                        .AddModbusRtuDevice()
                            .AddSubscription(Modbus.HoldingRegister(0, 0, Codec.Int16), Trigger.Interval(10))
                            .OnValue(x => { })
                    .AddTcpPort(IPAddress.Loopback, 8086)
                        .AddModbusIpDevice()
                            .AddSubscription(Modbus.InputRegister(1, 0, Codec.Float), Trigger.Interval(100))
                )
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

