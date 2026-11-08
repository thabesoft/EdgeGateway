using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.EdgeGateway.Lifecycles.Builder;

namespace ThabeSoft.EdgeGateway.Lifecycles;


public static class Lifecycle
{
    /// <summary>
    /// 默认值 Start:30s, Stop:30s, Dispose:10s, Lock:30s
    /// </summary>
    public static LifecycleStateMachineA Normal(Action<NormalLifecycleTimeoutBuilder>? action = null, ILogger? logger = null)
    {
        var builder = new NormalLifecycleTimeoutBuilder();
        action?.Invoke(builder);

        return new(logger ?? NullLogger.Instance)
        {
            StartTimeout = builder.StartTimeout,
            StopTimeout = builder.StopTimeout,
            DisposeTimeout = builder.DisposeTimeout,
            LockTimeout = builder.LockTimeout,
        };
    }

    // 默认值 
    public static CollectionLifecycleStateMachine Collection(Action<CollectionLifecycleStateMachineBuilder>? action = null, ILogger? logger = null)
    {
        var builder = new CollectionLifecycleStateMachineBuilder();
        action?.Invoke(builder);

        var innerStateMachine = Normal(x => x
            .Start(builder.StartTimeout)
            .Stop(builder.StopTimeout)
            .Dispose(builder.DisposeTimeout)
            .Lock(builder.LockTimeout));

        return new(logger ?? NullLogger.Instance)
        {
            StartTimeout = builder.StartTimeout,
            StopTimeout = builder.StopTimeout,
            DisposeTimeout = builder.DisposeTimeout,
            LockTimeout = builder.LockTimeout,

            ItemStartTimeout = builder.ItemStartTimeout,
            ItemStopTimeout = builder.ItemStopTimeout,
            ItemDisposeTimeout = builder.ItemDisposeTimeout
        };
    }


    public static ILifecycle Disposer(Func<ValueTask> callback) => new CallbackLifecycle() { OnDispose = callback };
    public static ILifecycle Starter(Func<CancellationToken, ValueTask> callback) => new CallbackLifecycle() { OnStart = callback };
    public static ILifecycle Stoper(Func<CancellationToken, ValueTask> callback) => new CallbackLifecycle() { OnStop = callback };
}
