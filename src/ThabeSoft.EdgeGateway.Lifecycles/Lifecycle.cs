using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;

namespace ThabeSoft.EdgeGateway;


public static class Lifecycle
{
    #region --生命周期--

    /// <summary>
    /// 使用委托创建生命周期
    /// </summary>
    public static IDisposableLifecycle Background(Func<CancellationToken, Task> backgroundAction, ILogger? logger = null)
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

    // 批量执行器
    public static ILifecycleBatchExecutor BatchExecutor(ILogger? logger = null) =>
        new LifecycleBatchExecutor(logger);
}