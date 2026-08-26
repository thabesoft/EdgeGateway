using EdgeGateway.Devices;
using FluentResults;

namespace EdgeGateway.Ports;


public interface IPortHandle : IPort
{
    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }


    Result<IDevice> AddDevice(DeviceOptions config);
    ValueTask<Result> RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken = default);
}