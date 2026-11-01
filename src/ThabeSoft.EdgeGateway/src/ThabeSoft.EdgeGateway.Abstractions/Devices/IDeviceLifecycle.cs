namespace ThabeSoft.EdgeGateway.Devices;

public interface IDeviceLifecycle
{
    bool IsEnabled { get; }

    ValueTask EnableAsync(CancellationToken cancellationToken = default);
    ValueTask DisableAsync(CancellationToken cancellationToken = default);
}