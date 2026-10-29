using EdgeGateway.Protocol;
using EdgeGateway.Tag;

namespace EdgeGateway.Device;


internal sealed class DefaultDevice(ITagReader tagReader) : IDevice
{
    public required DeviceId Id { get; init; }
    public required ProtocolType ProtocolType { get; init; }
    public bool IsEnabled { get; private set; }


    public ValueTask EnableAsync(CancellationToken cancellationToken = default)
    {
        IsEnabled = true;
        return ValueTask.CompletedTask;
    }
    public ValueTask DisableAsync(CancellationToken cancellationToken = default)
    {
        IsEnabled = false;
        return ValueTask.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}
