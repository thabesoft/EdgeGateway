using EdgeGateway.Devices;
using EdgeGateway.Ports;
using FluentResults;

namespace EdgeGateway.Tags;


internal sealed class TagReader(IPortManager portManager, IDeviceManager deviceManager) : ITagReader
{
    public ValueTask<Result<T>> ReadAsync<T>(Tag<T> tag, CancellationToken cancellationToken)
    {
        if (!portManager.Ports.TryGetValue(tag.PortAddress, out var portHandle))
        {
            return ValueTask.FromResult(Result.Fail<T>($"未找到会话 {tag.PortAddress}"));
        }

        if (!deviceManager.Devices.TryGetValue(tag.DeviceId, out var device))
        {
            return ValueTask.FromResult(Result.Fail<T>($"未找到设备 {tag.DeviceId}"));
        }

        return device.ReadAsync(tag.DataDescriptor, cancellationToken);
    }
}
