namespace ThabeSoft.EdgeGateway.Subscription;


public readonly struct Reading
{
    public object Value { get; }
    public ReadingQuality Quality { get; }
    public DateTimeOffset Timestamp { get; }


    internal Reading(object value, ReadingQuality quality, DateTimeOffset timestamp)
    {
        Value = value;
        Quality = quality;
        Timestamp = timestamp;
    }
    public override string ToString() => $"Reading({Value}, {Quality}, {Timestamp:HH:mm:ss.fff})";


    public static Reading<T> Create<T>(T value, ReadingQuality quality, DateTimeOffset timestamp) where T: notnull
    {
        ArgumentNullException.ThrowIfNull(value);
        if (!Enum.IsDefined(quality))
            throw new ArgumentOutOfRangeException(nameof(quality), quality, "不支持的质量类型");

        return new(value, quality, timestamp);
    }
    public static Reading<T> Create<T>(T value, ReadingQuality quality) where T : notnull =>
        Create(value, quality, DateTimeOffset.UtcNow);


    public static Reading<T> Good<T>(T value) where T : notnull =>
        Create(value, ReadingQuality.Good);
    public static Reading<T> Bad<T>(T value) where T : notnull =>
        Create(value, ReadingQuality.Bad);
}



public readonly struct Reading<T> where T : notnull
{
    public DateTimeOffset Timestamp { get; }
    public ReadingQuality Quality { get; }
    public T Value { get; }


    internal Reading(T value, ReadingQuality quality, DateTimeOffset timestamp)
    {
        Value = value;
        Quality = quality;
        Timestamp = timestamp;
    }
    public override string ToString() => $"Reading({Value}, {Quality}, {Timestamp:HH:mm:ss.fff})";


    public static implicit operator Reading(Reading<T> reading) =>
        new(reading.Value, reading.Quality, reading.Timestamp);
}