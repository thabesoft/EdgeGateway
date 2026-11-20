using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.Lifecycles;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Transports;

namespace ThabeSoft.EdgeGateway.Ports;


public interface IPort : ILifecycleWithState
{
    PortId Id { get; }
    ConnectionEndpoint Endpoint { get; }
    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }

    ValueTask<IDevice> AddDeviceAsync(DeviceDescriptor deviceDescriptor, CancellationToken cancellationToken = default);
    ValueTask<bool> RemoveDeviceAsync(DeviceId deviceId, CancellationToken cancellationToken = default);
}


public static class PortExtensions
{
    extension(IPort port)
    {
        public ValueTask<IDevice> AddDeviceAsync(DeviceId deviceId, ProtocolDescriptor protocol, CancellationToken cancellationToken = default) =>
            port.AddDeviceAsync(new DeviceDescriptor() { Id = deviceId, Protocol = protocol }, cancellationToken);
        public ValueTask<IDevice> AddDeviceAsync(ProtocolDescriptor protocol, CancellationToken cancellationToken = default) =>
            port.AddDeviceAsync(DeviceId.Create(), protocol, cancellationToken);
    }
}