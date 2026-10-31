namespace EdgeGateway.Device;

public interface IDeviceLifecycle
{
    bool IsEnabled { get; }

    ValueTask EnableAsync(CancellationToken cancellationToken = default);
    ValueTask DisableAsync(CancellationToken cancellationToken = default);
}