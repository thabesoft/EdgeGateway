using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway;


public static class Lifecycle
{
    #region --生命周期--

    /// <summary>
    /// 使用委托创建生命周期
    /// </summary>
    public static IBackgroundLifecycle Background(Func<CancellationToken, Task> backgroundAction, ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(backgroundAction);
        return new BackgroundLifecycle(backgroundAction, logger);
    }

    /// <summary>
    /// 带键的生命周集合
    /// </summary>
    public static ILifecycleKeyedCollection<TKey, TLifecycle> KeyedCollection<TKey, TLifecycle>(ILifecycleStateable lifecycleStateable, ILogger? logger = null)
        where TKey : notnull
        where TLifecycle : ILifecycle, IAsyncDisposable
    {
        ArgumentNullException.ThrowIfNull(lifecycleStateable);
        return new LifecycleKeyedCollection<TKey, TLifecycle>(lifecycleStateable, logger);
    }

    #endregion

    #region --状态机--

    /// <summary>
    /// 标准 启动->停止->释放 状态机
    /// </summary>
    public static IStrictLifecycleStateMachine StrictStateMachine(ILogger? logger = null) =>
        new StrictLifecycleStateMachine(logger);

    /// <summary>
    /// 可以连续重复启动的状态机
    /// </summary>
    public static IRetryableLifecycleStateMachine RetryableStateMachine(ILogger? logger = null) =>
        new RetryableLifecycleStateMachine(logger);

    #endregion


    // 批量执行器
    public static ILifecycleBatchExecutor BatchExecutor(ILogger? logger = null) =>
        new LifecycleBatchExecutor(logger);
}