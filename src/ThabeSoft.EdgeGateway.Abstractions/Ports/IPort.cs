using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;


public interface IPort : ILifecycleWithState
{
    ConnectionEndpoint Endpoint { get; }
    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }

    ValueTask<IDevice> AddDeviceAsync(DeviceDescriptor deviceDescriptor, CancellationToken cancellationToken = default);
    ValueTask<bool> RemoveDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default);
}