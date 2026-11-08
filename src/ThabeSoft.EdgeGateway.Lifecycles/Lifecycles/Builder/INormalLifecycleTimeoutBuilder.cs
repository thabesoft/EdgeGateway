namespace ThabeSoft.EdgeGateway.Lifecycles.Builder;

public interface INormalLifecycleTimeoutBuilder<TSelf>
    where TSelf : INormalLifecycleTimeoutBuilder<TSelf>
{
    TSelf Start(TimeSpan timeout);
    TSelf Stop(TimeSpan timeout);
    TSelf Dispose(TimeSpan timeout);
    TSelf Lock(TimeSpan timeout);
}


public sealed class NormalLifecycleTimeoutBuilder : INormalLifecycleTimeoutBuilder<NormalLifecycleTimeoutBuilder>
{
    internal TimeSpan StartTimeout { get; private set; } = TimeSpan.FromSeconds(30);
    internal TimeSpan StopTimeout { get; private set; } = TimeSpan.FromSeconds(30);
    internal TimeSpan DisposeTimeout { get; private set; } = TimeSpan.FromSeconds(10);
    internal TimeSpan LockTimeout { get; private set; } = TimeSpan.FromSeconds(30);



    public NormalLifecycleTimeoutBuilder Start(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeout.TotalSeconds);
        StartTimeout = timeout;
        return this;
    }
    public NormalLifecycleTimeoutBuilder Stop(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeout.TotalSeconds);
        StopTimeout = timeout;
        return this;
    }
    public NormalLifecycleTimeoutBuilder Dispose(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeout.TotalSeconds);
        DisposeTimeout = timeout;
        return this;
    }
    public NormalLifecycleTimeoutBuilder Lock(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeout.TotalSeconds);
        LockTimeout = timeout;
        return this;
    }
}