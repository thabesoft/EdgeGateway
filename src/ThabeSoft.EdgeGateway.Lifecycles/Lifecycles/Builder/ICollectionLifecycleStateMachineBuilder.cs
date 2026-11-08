namespace ThabeSoft.EdgeGateway.Lifecycles.Builder;

public interface ICollectionLifecycleStateMachineBuilder<TSelf> : INormalLifecycleTimeoutBuilder<TSelf>
    where TSelf : ICollectionLifecycleStateMachineBuilder<TSelf>
{
    TSelf ItemStart(TimeSpan timeout);
    TSelf ItemStop(TimeSpan timeout);
    TSelf ItemDispose(TimeSpan timeout);
}

public sealed class CollectionLifecycleStateMachineBuilder : ICollectionLifecycleStateMachineBuilder<CollectionLifecycleStateMachineBuilder>
{
    internal TimeSpan StartTimeout { get; private set; } = TimeSpan.FromSeconds(30);
    internal TimeSpan StopTimeout { get; private set; } = TimeSpan.FromSeconds(30);
    internal TimeSpan DisposeTimeout { get; private set; } = TimeSpan.FromSeconds(10);
    internal TimeSpan LockTimeout { get; private set; } = TimeSpan.FromSeconds(30);

    internal TimeSpan ItemStartTimeout { get; private set; } = TimeSpan.FromSeconds(10);
    internal TimeSpan ItemStopTimeout { get; private set; } = TimeSpan.FromSeconds(10);
    internal TimeSpan ItemDisposeTimeout { get; private set; } = TimeSpan.FromSeconds(10);


    public CollectionLifecycleStateMachineBuilder Start(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeout.TotalSeconds);
        StartTimeout = timeout;
        return this;
    }

    public CollectionLifecycleStateMachineBuilder Stop(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeout.TotalSeconds);
        StopTimeout = timeout;
        return this;
    }
    public CollectionLifecycleStateMachineBuilder Dispose(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeout.TotalSeconds);
        DisposeTimeout = timeout;
        return this;
    }
    public CollectionLifecycleStateMachineBuilder Lock(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeout.TotalSeconds);
        LockTimeout = timeout;
        return this;
    }


    public CollectionLifecycleStateMachineBuilder ItemStart(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeout.TotalSeconds);
        ItemStartTimeout = timeout;
        return this;
    }

    public CollectionLifecycleStateMachineBuilder ItemStop(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeout.TotalSeconds);
        ItemStartTimeout = timeout;
        return this;
    }
    public CollectionLifecycleStateMachineBuilder ItemDispose(TimeSpan timeout)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(timeout.TotalSeconds);
        ItemDisposeTimeout = timeout;
        return this;
    }
}


