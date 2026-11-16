using ThabeSoft.EdgeGateway.Compatibility;

namespace ThabeSoft.EdgeGateway.Transports;


public abstract record class TransportDescriptor
{
    public abstract ConnectionEndpoint Endpoint { get; }
    public RetryDescriptor Retry { get; init => field = value.Required(); } = Transports.Retry.Default;
}


public sealed record class RetryDescriptor
{
    public RetryDescriptor(TimeSpan startInterval, TimeSpan maxInterval)
    {
        if (startInterval <= TimeSpan.Zero)
            throw new ArgumentException("重试间隔必须大于0", nameof(startInterval));

        if (maxInterval < startInterval)
            throw new ArgumentException("最大间隔不得超过起始间隔", nameof(maxInterval));

        StartInterval = startInterval;
        MaxInterval = maxInterval;
    }

    // 重试间隔
    public TimeSpan StartInterval { get; }
    // 最大重试间隔
    public TimeSpan MaxInterval { get; }
    // 下一次重试退避倍数
    public required double BackoffMultiplier { get; init => field = value.AtLeast(1); }


    public override string ToString()
    {
        return $"Retry({StartInterval.TotalSeconds:g}s~{MaxInterval.TotalSeconds:g}s:{BackoffMultiplier:g})";
    }
}


public static class Retry
{
    public static RetryDescriptor Default { get; } = From(TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(30), 1.5);

    public static RetryDescriptor From(TimeSpan startInterval, TimeSpan maxInterval, double backoffMultiplier) =>
        new(startInterval, maxInterval) { BackoffMultiplier = backoffMultiplier };
    public static RetryDescriptor From(double startIntervalMilliseconds, double maxIntervalMilliseconds, double backoffMultiplier) =>
        From(TimeSpan.FromMilliseconds(startIntervalMilliseconds), TimeSpan.FromMilliseconds(maxIntervalMilliseconds), backoffMultiplier);
}