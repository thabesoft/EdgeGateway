using EdgeGateway.Ports;
using FluentResults;

namespace EdgeGateway.Devices;

public interface IDeviceManager
{
    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }


    Result<IDevice> Add(IPort port, DeviceOptions options);
    ValueTask<Result> RemovAsync(DeviceId id, CancellationToken cancellationToken = default);
}