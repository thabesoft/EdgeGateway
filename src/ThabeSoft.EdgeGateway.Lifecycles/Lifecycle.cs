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
        builder.WithItemStartTimeout(5000).WithItemStartTimeout(3000).WithItemDisposeTimeout(3000).WithMaxConcurrency(3);
        builderAction?.Invoke(builder);

        return new LifecycleBatchExecutorDescriptor()
        {
            ItemStartTimeout = builder.ItemStartTimeout,
            ItemStopTimeout = builder.ItemStopTimeout,
            ItemDisposeTimeout = builder.ItemDisposeTimeout,
            Concurrency = builder.MaxConcurrency
        };
    }

    public static LifecycleStateMachineDescriptor RetryableStateMachine(Action<LifecycleStateMachineDescriptorConfigurator>? builderAction = null)
    {
        var builder = new LifecycleStateMachineDescriptorConfigurator();
        builder.WithStartTimeout(30000).WithStopTimeout(15000).WithDisposeTimeout(5000).WithExecuteTimeout(3000);
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
        builder.WithStartTimeout(30000).WithStopTimeout(15000).WithDisposeTimeout(5000).WithExecuteTimeout(3000);
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


    public LifecycleStateMachineDescriptorConfigurator WithStartTimeout(TimeSpan timeout) { StartTimeout = timeout; return this; }
    public LifecycleStateMachineDescriptorConfigurator WithStartTimeout(double timeoutMs) => WithStartTimeout(TimeSpan.FromMilliseconds(timeoutMs));


    public LifecycleStateMachineDescriptorConfigurator WithStopTimeout(TimeSpan timeout) { StopTimeout = timeout; return this; }
    public LifecycleStateMachineDescriptorConfigurator WithStopTimeout(double timeoutMs) => WithStopTimeout(TimeSpan.FromMilliseconds(timeoutMs));


    public LifecycleStateMachineDescriptorConfigurator WithDisposeTimeout(TimeSpan timeout) { DisposeTimeout = timeout; return this; }
    public LifecycleStateMachineDescriptorConfigurator WithDisposeTimeout(double timeoutMs) => WithDisposeTimeout(TimeSpan.FromMilliseconds(timeoutMs));


    public LifecycleStateMachineDescriptorConfigurator WithExecuteTimeout(TimeSpan timeout) { ExecuteTimeout = timeout; return this; }
    public LifecycleStateMachineDescriptorConfigurator WithExecuteTimeout(double timeoutMs) => WithExecuteTimeout(TimeSpan.FromMilliseconds(timeoutMs));
}



public sealed class LifecycleBatchExecutorDescriptorConfigurator
{
    internal TimeSpan ItemStartTimeout { get; private set => field = value.Positive(); }
    internal TimeSpan ItemStopTimeout { get; private set => field = value.Positive(); }
    internal TimeSpan ItemDisposeTimeout { get; private set => field = value.Positive(); }
    internal int MaxConcurrency { get; private set => field = value.Positive(); }


    public LifecycleBatchExecutorDescriptorConfigurator WithItemStartTimeout(TimeSpan timeout) { ItemStartTimeout = timeout; return this; }
    public LifecycleBatchExecutorDescriptorConfigurator WithItemStartTimeout(double timeoutMs) => WithItemStartTimeout(TimeSpan.FromMilliseconds(timeoutMs));


    public LifecycleBatchExecutorDescriptorConfigurator WithItemStopTimeout(TimeSpan timeout) { ItemStartTimeout = timeout; return this; }
    public LifecycleBatchExecutorDescriptorConfigurator WithItemStopTimeout(double timeoutMs) => WithItemStopTimeout(TimeSpan.FromMilliseconds(timeoutMs));


    public LifecycleBatchExecutorDescriptorConfigurator WithItemDisposeTimeout(TimeSpan timeout) { ItemStartTimeout = timeout; return this; }
    public LifecycleBatchExecutorDescriptorConfigurator WithItemDisposeTimeout(double timeoutMs) => WithItemDisposeTimeout(TimeSpan.FromMilliseconds(timeoutMs));


    public LifecycleBatchExecutorDescriptorConfigurator WithMaxConcurrency(int count) { MaxConcurrency = count; return this; }
}