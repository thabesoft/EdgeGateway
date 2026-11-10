namespace ThabeSoft.EdgeGateway.Lifecycles;

/// <summary>
/// 生命周期
/// </summary>
public interface ILifecycle
{
    ValueTask StartAsync(CancellationToken cancellationToken = default);
    ValueTask StopAsync(CancellationToken cancellationToken = default);
}
