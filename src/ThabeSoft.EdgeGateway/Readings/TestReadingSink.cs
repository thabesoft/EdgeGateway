using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Readings;


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
        public required ReadingQuality Quality { get; init => field = value.Defined("不支持的质量类型"); }
        public required object Value { get; init => field = value.Required("值不可为空"); }


        public override string ToString()
        {
            return $"Data(Port({PortId}), Device({DeviceId}), Address({DataAddress}), Value({Value:F0}), Quality({Quality}), Time({Timestamp.LocalDateTime:HH:mm:ss:fff}))";
        }
    }
}