using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;
using ThabeSoft.EdgeGateway.Codecs;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Polling;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway;

[TestClass]
public class GatewayBenchmarks
{
    [TestMethod]
    public async Task EndToEnd_10msPolling_MeasuresLatency()
    {
        using var host = Host.CreateDefaultBuilder()
            .ConfigureServices(x => x
                .AddEdgeGateway()
                .AddModbus()
                .RemoveAll<ITransportFactory>()
                .AddSingleton<ITransportFactory, MockTransportFactory>())
            .Build();

        var gateway = host.Services.GetRequiredService<IGateway>();
        var port = await gateway.AddPortAsync(SerialTransportDescriptor.Create("COM2"));
        var device = await port.AddDeviceAsync(ModbusProtocolDescriptor.Rtu);
        var subscription = await device.AddSubscriptionAsync(
            SubscriptionDescriptor.Create(
                ModbusTags.HoldingRegister(1, 0, WordCodec.BigEndian),
                FixedIntervalTriggerDescriptor.FromMilliseconds(10)));

        var ticks = new ConcurrentQueue<long>();
        var count = 0;
        var last = Stopwatch.GetTimestamp();

        // 订阅保持存活（using 作用域覆盖整个测量期）
        using var sub = subscription.Tag.Subscribe(_ =>
        {
            var now = Stopwatch.GetTimestamp();
            ticks.Enqueue(now - Interlocked.Exchange(ref last, now));
            Interlocked.Increment(ref count);
        });

        await host.StartAsync(CancellationToken.None);

        // 预热：排除首次连接/JIT/定时器对齐
        await Task.Delay(1000);
        ticks.Clear();
        Interlocked.Exchange(ref count, 0);
        Interlocked.Exchange(ref last, Stopwatch.GetTimestamp());

        // 正式测量 10 秒
        await Task.Delay(TimeSpan.FromSeconds(10));
        await host.StopAsync(CancellationToken.None);

        var freq = (double)Stopwatch.Frequency;
        var samples = ticks.Select(t => t * 1000.0 / freq).OrderBy(x => x).ToArray();

        var p50 = samples[(int)(samples.Length * 0.50)];
        var p95 = samples[(int)(samples.Length * 0.95)];
        var p99 = samples[(int)(samples.Length * 0.99)];
        var max = samples[^1];

        TestContext.WriteLine($"触发 {count} 次");
        TestContext.WriteLine($"P50={p50:F2}ms, P95={p95:F2}ms, P99={p99:F2}ms, Max={max:F2}ms");
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
        private static readonly byte[] Response = [0x01, 0x03, 0x02, 0x00, 0x7B, 0xF8, 0x67];
        private int _offset;

        public ConnectionEndpoint Endpoint => transportDescriptor.Endpoint;

        public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            logger.LogDebug("读取数据, offset={Offset}, buffer={Length}", _offset, buffer.Length);

            if (_offset >= Response.Length)
            {
                logger.LogWarning("响应已读完仍被 ReadAsync 调用，offset={Offset}", _offset);
                return 0;
            }

            var count = Math.Min(buffer.Length, Response.Length - _offset);
            Response.AsMemory(_offset, count).CopyTo(buffer);
            _offset += count;
            return count;
        }
        public async ValueTask WriteAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken = default)
        {
            _offset = 0;
            logger.LogDebug("写入数据: {Length} bytes", data.Length);
        }

        protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("已启动");
        }
        protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
        {
            logger.LogDebug("已停止");
        }
    }
}