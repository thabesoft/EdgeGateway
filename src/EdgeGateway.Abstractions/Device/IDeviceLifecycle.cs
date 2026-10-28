namespace EdgeGateway.Device;

public interface IDeviceLifecycle
{
    bool IsEnabled { get; }

    ValueTask EnableAsync(CancellationToken ct = default);
    ValueTask DisableAsync(CancellationToken ct = default);
}