using EdgeGateway.Devices;
using EdgeGateway.Ports;
using FluentResults;

namespace EdgeGateway.Tags;


internal sealed class TagWriter(IPortManager portManager, IDeviceManager deviceManager) : ITagWriter
{
    public ValueTask<Result> WriteAsync<T>(Tag<T> tag, T value, CancellationToken cancellationToken)
    {
        if (!portManager.Ports.TryGetValue(tag.PortAddress, out var portHandle))
        {
            return ValueTask.FromResult(Result.Fail($"未找到会话 {tag.PortAddress}"));
        }

        if (!deviceManager.Devices.TryGetValue(tag.DeviceId, out var device))
        {
            return ValueTask.FromResult(Result.Fail($"未找到设备 {tag.DeviceId}"));
        }


        return device.WriteAsync(tag.DataDescriptor, value, cancellationToken);
    }
}