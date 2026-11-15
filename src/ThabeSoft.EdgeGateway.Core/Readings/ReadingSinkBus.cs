using Microsoft.Extensions.Logging;
using System.Threading.Channels;
using ThabeSoft.EdgeGateway.Addressing;
using ThabeSoft.EdgeGateway.Compatibility;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Readings;


internal sealed class ReadingSinkBus : IAsyncDisposable
{
    private readonly Channel<IWriteCommand> _channel = Channel.CreateBounded<IWriteCommand>(100000);
    private readonly IReadingSink[] _readingSinks;
    private readonly ILogger<ReadingSinkBus> _logger;


    private readonly Task _consumeTask;
    private readonly CancellationTokenSource _consumeTaskCts;


    public ReadingSinkBus(IEnumerable<IReadingSink> readingSinks, ILogger<ReadingSinkBus> logger)
    {
        _readingSinks = readingSinks.Where(x => x is not null).Distinct().ToArray();
        _logger = logger;

        _consumeTaskCts = new CancellationTokenSource();
        _consumeTask = Task.Run(async () => await ConsumerAsync(_consumeTaskCts.Token), _consumeTaskCts.Token);
    }
    public void Publish<T>(ReadingContext context, Reading<T> reading)
        where T : notnull
    {
        var cmd = new WriteCommand<T>(context, reading);
        _channel.Writer.TryWrite(cmd);
    }
    public async ValueTask DisposeAsync()
    {
        await _consumeTaskCts.CancelAsync();
        try
        {
            await _consumeTask;
        }
        catch(OperationCanceledException) when(_consumeTaskCts.IsCancellationRequested)
        {

        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "释放失败");
        }
    }



    private async ValueTask ConsumerAsync(CancellationToken cancellationToken)
    {
        var sinks = _readingSinks.ToArray();

        await foreach (var cmd in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            if (sinks.Length == 1)
            {
                await SafeExecuteAsync(cmd, sinks[0], cancellationToken);
                continue;
            }


            var tasks = new Task[sinks.Length];
            for (int i = 0; i < sinks.Length; i++)
            {
                tasks[i] = SafeExecuteAsync(cmd, sinks[i], cancellationToken);
            }
            await Task.WhenAll(tasks);
        }
    }

    private async Task SafeExecuteAsync(IWriteCommand cmd, IReadingSink sink, CancellationToken ct)
    {
        try
        {
            await cmd.ExecuteAsync(sink, ct);
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "数据写入 Sink 失败: {SinkType}", sink.GetType().Name);
        }
    }


    private interface IWriteCommand
    {
        ValueTask ExecuteAsync(IReadingSink readingSink, CancellationToken cancellationToken);
    }
    private readonly struct WriteCommand<T>(ReadingContext context, Reading<T> reading) : IWriteCommand
        where T : notnull
    {
        ValueTask IWriteCommand.ExecuteAsync(IReadingSink readingSink, CancellationToken cancellationToken)
        {
            return readingSink.WriteAsync(context, reading, cancellationToken);
        }
    }
}


internal sealed class TestReadingSink(ILogger<TestReadingSink> logger) : IReadingSink
{
    public async ValueTask WriteAsync<T>(ReadingContext context, Reading<T> reading, CancellationToken cancellationToken = default) where T : notnull
    {
        // 假设由100ms 延时
        await Task.Delay(Random.Shared.Next(100, 1500));

        var datapoint = new DataPoint()
        {
            PortId = context.PortId,
            DeviceId = context.DeviceId,
            DataAddress = context.DataAddress,
            Timestamp = reading.Timestamp,
            Quality = reading.Quality,
            Value = reading.Value
        };

        logger.LogInformation("数据点储存({Value})", datapoint);
    }


    public readonly record struct DataPoint
    {
        public required PortId PortId { get; init => field = value.Required(); }
        public required DeviceId DeviceId { get; init => field = value.Required(); }
        public required DataAddress DataAddress { get; init => field = value.Required(); }
        public required DateTimeOffset Timestamp { get; init => field = value.NotDefault("时间戳异常"); }
        public required ReadingQuality Quality { get; init => field.Defined("不支持的质量类型"); }
        public required object Value { get; init => field = value.Required("值不可为空"); }
    }
}