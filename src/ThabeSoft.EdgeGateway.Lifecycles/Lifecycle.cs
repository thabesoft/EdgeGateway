using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway;


public static class Lifecycle
{
    /// <summary>
    /// 使用委托创建生命周期
    /// </summary>
    public static IDisposableLifecycle Background(Func<CancellationToken, Task> backgroundAction, ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(backgroundAction);
        return new BackgroundLifecycle(backgroundAction, logger);
    }


    public static LifecycleBatchExecutorDescriptor BatchExecutor(Action<LifecycleBatchExecutorDescriptorConfigurator>? builderAction = null)
    {
        var builder = new LifecycleBatchExecutorDescriptorConfigurator();
        builder.ItemStartTimeout(5000).ItemStartTimeout(3000).ItemDisposeTimeout(3000).MaxConcurrency(3);
        builderAction?.Invoke(builder);

        return new LifecycleBatchExecutorDescriptor()
        {
            ItemStartTimeout = builder.StartTimeout,
            ItemStopTimeout = builder.StopTimeout,
            ItemDisposeTimeout = builder.DisposeTimeout,
            Concurrency = builder.Concurrency
        };
    }

    public static LifecycleStateMachineDescriptor RetryableStateMachine(Action<LifecycleStateMachineDescriptorConfigurator>? builderAction = null)
    {
        var builder = new LifecycleStateMachineDescriptorConfigurator();
        builder.Start(30000).Stop(15000).Dispose(5000).Execute(3000);
        builderAction?.Invoke(builder);

        return new RetryableLifecycleStateMachineDescriptor()
        {
            StartTimeout = builder.StartTimeout,
            StopTimeout = builder.StopTimeout,
            DisposedTimeout = builder.DisposeTimeout,
            ExecuteTimeout = builder.ExecuteTimeout
        };
    }
    public static LifecycleStateMachineDescriptor StrictStateMachine(Action<LifecycleStateMachineDescriptorConfigurator>? builderAction = null)
    {
        var builder = new LifecycleStateMachineDescriptorConfigurator();
        builder.Start(30000).Stop(15000).Dispose(5000).Execute(3000);
        builderAction?.Invoke(builder);

        return new StrictLifecycleStateMachineDescriptor()
        {
            StartTimeout = builder.StartTimeout,
            StopTimeout = builder.StopTimeout,
            DisposedTimeout = builder.DisposeTimeout,
            ExecuteTimeout = builder.ExecuteTimeout
        };
    }
}


public sealed class LifecycleStateMachineDescriptorConfigurator
{
    internal TimeSpan StartTimeout { get; private set => field = value.Positive(); }
    internal TimeSpan StopTimeout { get; private set => field = value.Positive(); }
    internal TimeSpan DisposeTimeout { get; private set => field = value.Positive(); }
    internal TimeSpan ExecuteTimeout { get; private set => field = value.Positive(); }


    public LifecycleStateMachineDescriptorConfigurator Start(TimeSpan timeout) { StartTimeout = timeout; return this; }
    public LifecycleStateMachineDescriptorConfigurator Start(double timeoutMs) => Start(TimeSpan.FromMilliseconds(timeoutMs));


    public LifecycleStateMachineDescriptorConfigurator Stop(TimeSpan timeout) { StartTimeout = timeout; return this; }
    public LifecycleStateMachineDescriptorConfigurator Stop(double timeoutMs) => Stop(TimeSpan.FromMilliseconds(timeoutMs));


    public LifecycleStateMachineDescriptorConfigurator Dispose(TimeSpan timeout) { StartTimeout = timeout; return this; }
    public LifecycleStateMachineDescriptorConfigurator Dispose(double timeoutMs) => Dispose(TimeSpan.FromMilliseconds(timeoutMs));


    public LifecycleStateMachineDescriptorConfigurator Execute(TimeSpan timeout) { ExecuteTimeout = timeout; return this; }
    public LifecycleStateMachineDescriptorConfigurator Execute(double timeoutMs) => Execute(TimeSpan.FromMilliseconds(timeoutMs));
}



public sealed class LifecycleBatchExecutorDescriptorConfigurator
{
    internal TimeSpan StartTimeout { get; private set => field = value.Positive(); }
    internal TimeSpan StopTimeout { get; private set => field = value.Positive(); }
    internal TimeSpan DisposeTimeout { get; private set => field = value.Positive(); }
    internal int Concurrency { get; private set => field = value.Positive(); }


    public LifecycleBatchExecutorDescriptorConfigurator ItemStartTimeout(TimeSpan timeout) { StartTimeout = timeout; return this; }
    public LifecycleBatchExecutorDescriptorConfigurator ItemStartTimeout(double timeoutMs) => ItemStartTimeout(TimeSpan.FromMilliseconds(timeoutMs));


    public LifecycleBatchExecutorDescriptorConfigurator ItemStopTimeout(TimeSpan timeout) { StartTimeout = timeout; return this; }
    public LifecycleBatchExecutorDescriptorConfigurator ItemStopTimeout(double timeoutMs) => ItemStopTimeout(TimeSpan.FromMilliseconds(timeoutMs));


    public LifecycleBatchExecutorDescriptorConfigurator ItemDisposeTimeout(TimeSpan timeout) { StartTimeout = timeout; return this; }
    public LifecycleBatchExecutorDescriptorConfigurator ItemDisposeTimeout(double timeoutMs) => ItemDisposeTimeout(TimeSpan.FromMilliseconds(timeoutMs));


    public LifecycleBatchExecutorDescriptorConfigurator MaxConcurrency(int count) { Concurrency = count; return this; }
}