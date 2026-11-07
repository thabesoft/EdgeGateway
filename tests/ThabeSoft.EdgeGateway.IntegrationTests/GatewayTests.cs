using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using System.Net;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.Simulator.ModbusSlave;

namespace ThabeSoft.EdgeGateway;


[TestClass]
public class GatewayTests
{
    [TestMethod]
    public async Task Normal()
    {
        var fakeTimeProvider = new FakeTimeProvider();

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
                .AddSingleton<TimeProvider>(fakeTimeProvider)
                //.RemoveAll<ITransportFactory>()
                //.AddSingleton<ITransportFactory, MockTransportFactory>()
                )
            .Build();

        var serialSimulator = app.Services.GetRequiredService<IModbusSerialSimulator>();
        serialSimulator.SetHoldingRegister(0, 123);


        var gateway = app.Services.GetRequiredService<IGateway>();


        var port = gateway.AddPort(Serial.Port("COM2"));
        var device = port.AddDevice(Modbus.Rtu);

        var tag = Modbus.HoldingRegister(1, 0, WordCodec.BigEndian.AsShort());
        var subscription = device.AddSubscription(tag, Trigger.Interval(100));

        var tcs = new TaskCompletionSource();
        using var _ = subscription.Subscribe(x =>
        {
            TestContext.WriteLine(x.ToString());
            tcs.SetResult();
        });

        await app.StartAsync(TestContext.CancellationToken);
        fakeTimeProvider.Advance(TimeSpan.FromSeconds(100));

        tcs.Task.Wait(TestContext.CancellationToken);
    }

    [TestMethod]
    public async Task FromBuilder()
    {
        var fakeTimeProvider = new FakeTimeProvider();

        using var app = Host.CreateDefaultBuilder()
            .ConfigureServices(x => x
                .AddModbusRtuSimulator(x =>
                {
                    x.UnitId = 1;
                    x.SerialPortOptions = new SerialPortOptions() { PortName = "COM3" };
                })
                .AddEdgeGateway()
                .ConfigureGateway(gateway => gateway
                    .AddSerialPort("COM2")
                        .AddModbusRtuDevice()
                            .AddSubscription(Modbus.HoldingRegister(1, 0, Codec.Int16), Trigger.Interval(10))
                                .Subscribe(x => TestContext.WriteLine(x.ToString()))
                    .AddTcpPort(IPAddress.Loopback, 8086)
                        .AddModbusIpDevice()
                            .AddSubscription(Modbus.InputRegister(1, 10, Codec.Float), Trigger.Interval(100))
                )
                .AddModbus()
                .AddSerial()
                .AddEthernet()
                .AddSingleton<TimeProvider>(fakeTimeProvider)
                )
            .Build();

        var serialSimulator = app.Services.GetRequiredService<IModbusSerialSimulator>();
        serialSimulator.SetHoldingRegister(0, 123);
        serialSimulator.SetHoldingRegister(10, 456);


        var gateway = app.Services.GetRequiredService<IGateway>();
        await app.StartAsync(TestContext.CancellationToken);
        fakeTimeProvider.Advance(TimeSpan.FromSeconds(1));

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

