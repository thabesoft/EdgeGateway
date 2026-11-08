namespace ThabeSoft.EdgeGateway.Lifecycles;

public interface ILifecycle
{
    ValueTask StartAsync(CancellationToken cancellationToken = default);
    ValueTask StopAsync(CancellationToken cancellationToken = default);
}
