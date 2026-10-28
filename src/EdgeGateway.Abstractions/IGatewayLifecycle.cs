namespace EdgeGateway;

public interface IGatewayLifecycle
{
    bool IsRunning { get; }

    ValueTask StartAsync(CancellationToken cancellationToken = default);
    ValueTask StopAsync(CancellationToken cancellationToken = default);
}
