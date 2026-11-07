namespace ThabeSoft.EdgeGateway.Subscription;


public readonly record struct Reading
{
    public required DateTimeOffset Timestamp { get; init; }
    public required ReadingQuality Quality { get; init; }
    public required object Value { get; init; }


    public static Reading<T> Good<T>(T value) where T : notnull
    {
        return new()
        {
            Value = value,
            Quality = ReadingQuality.Good,
            Timestamp = DateTimeOffset.UtcNow
        };
    }
    public static Reading<T> Bad<T>() where T : notnull
    {
        return new()
        {
            Value = default!,
            Quality = ReadingQuality.Good,
            Timestamp = DateTimeOffset.UtcNow
        };
    }
}


public readonly record struct Reading<T> where T : notnull
{
    public required DateTimeOffset Timestamp { get; init; }
    public required ReadingQuality Quality { get; init; }
    public required T Value { get; init; }

    public static implicit operator Reading(Reading<T> reading) => new()
    {
        Value = reading.Value,
        Quality = reading.Quality,
        Timestamp = reading.Timestamp
    };
}