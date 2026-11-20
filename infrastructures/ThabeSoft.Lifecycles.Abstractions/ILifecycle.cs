namespace ThabeSoft.EdgeGateway.Lifecycles;

/// <summary>
/// 生命周期
/// </summary>
public interface ILifecycle
{
    ValueTask StartAsync(CancellationToken cancellationToken = default);
    ValueTask StopAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 可以释放的声明周期
/// </summary>
public interface IDisposableLifecycle : ILifecycle, IAsyncDisposable;