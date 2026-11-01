namespace ThabeSoft.EdgeGateway.Lifecycles;

public interface ILifecycle : IAsyncDisposable 
{
    LifecycleState State { get; }

    ValueTask StartAsync(CancellationToken cancellationToken = default);
    ValueTask StopAsync(CancellationToken cancellationToken = default);
}


/// <summary>
/// 组件的生命周期状态。
/// </summary>
public enum LifecycleState
{
    /// <summary>已创建，但从未启动过。</summary>
    Created,

    /// <summary>正在启动中。此时不能接受停止操作。</summary>
    Starting,

    /// <summary>已启动，正在运行。</summary>
    Running,

    /// <summary>正在停止中。此时不能接受启动操作。</summary>
    Stopping,

    /// <summary>已停止（可再次启动）。</summary>
    Stopped,

    /// <summary>已彻底释放，不可再启动或停止。</summary>
    Disposed
}