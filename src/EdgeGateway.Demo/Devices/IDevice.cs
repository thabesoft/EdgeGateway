using EdgeGateway.Tags;

namespace EdgeGateway.Devices;


public interface IDevice : ITagReader, ITagWriter
{
    DeviceId Id { get; }
    bool IsEnabled { get; }


    ValueTask EnableAsync(CancellationToken ct = default);
    ValueTask DisableAsync(CancellationToken ct = default);
}

public abstract class DeviceBase : IDevice
{
    public DeviceId Id { get; init; } = DeviceId.Create();
    public bool IsEnabled { get; private set; } = true;


    public ValueTask EnableAsync(CancellationToken ct = default)
    {
        IsEnabled = true;
        return ValueTask.CompletedTask;
    }
    public ValueTask DisableAsync(CancellationToken ct = default)
    {
        IsEnabled = false;
        return ValueTask.CompletedTask;
    }


    public ValueTask<T> ReadAsync<T>(ITag<T> tag, CancellationToken cancellationToken = default)
    {
        if (!IsEnabled)
            throw new InvalidOperationException("设备未启用");

        return ReadTagAsync(tag, cancellationToken);
    }
    public ValueTask WriteAsync<T>(ITag<T> tag, T value, CancellationToken cancellationToken = default)
    {
        if (!IsEnabled)
            throw new InvalidOperationException("设备未启用");

        return WriteTagAsync(tag, value, cancellationToken);
    }

    protected abstract ValueTask<T> ReadTagAsync<T>(ITag<T> tag, CancellationToken cancellationToken);
    protected abstract ValueTask WriteTagAsync<T>(ITag<T> tag, T value, CancellationToken cancellationToken);
}