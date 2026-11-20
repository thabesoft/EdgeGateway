using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Readings;


public readonly record struct Reading
{
    public required ReadingQuality Quality { get; init => field = value.Defined("不支持的质量类型"); }
    public required DateTimeOffset Timestamp { get; init => field = value.NotDefault("时间戳异常"); }
    public required object Value { get; init => field = value.Required("值不可为空"); }


    public override string ToString() => $"Reading<{Value.GetType().Name}>({Value}, {Quality}, {Timestamp:HH:mm:ss.fff})";



    public static Reading<T> Good<T>(T value) where T : notnull =>
        new() { Value = value, Quality = ReadingQuality.Good, Timestamp = DateTimeOffset.UtcNow };
    public static Reading<T> Bad<T>(T value) where T : notnull =>
        new() { Value = value, Quality = ReadingQuality.Bad, Timestamp = DateTimeOffset.UtcNow };
}



public readonly record struct Reading<T> where T : notnull
{
    public required DateTimeOffset Timestamp { get; init => field = value.NotDefault("时间戳异常"); }
    public required ReadingQuality Quality { get; init => field = value.Defined("不支持的质量类型"); }
    public required T Value { get; init => field = value.Required("值不可为空"); }


    public override string ToString() => $"Reading<{typeof(T).Name}>({Value}, {Quality}, {Timestamp.LocalDateTime:HH:mm:ss.fff})";


    public static implicit operator Reading(Reading<T> reading) =>
        new() { Value = reading.Value, Quality = reading.Quality, Timestamp = reading.Timestamp };
}