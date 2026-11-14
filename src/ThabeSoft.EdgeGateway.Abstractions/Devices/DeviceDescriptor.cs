using ThabeSoft.EdgeGateway.Compatibility;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Devices;


public sealed record class DeviceDescriptor
{
    public DeviceId Id { get; init => field = value.Required(); } = DeviceId.Create();
    public required ProtocolDescriptor Protocol { get; init => field = value.Required(); }
}


public static class PortExtensions
{
    extension(IPort port)
    {
        public ValueTask<IDevice> AddDeviceAsync(DeviceId deviceId, ProtocolDescriptor protocolDescriptor, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(deviceId);
            ArgumentNullException.ThrowIfNull(protocolDescriptor);


            return port.AddDeviceAsync(new() { Id = deviceId, Protocol = protocolDescriptor }, cancellationToken);
        }

        public ValueTask<IDevice> AddDeviceAsync(ProtocolDescriptor protocolDescriptor, CancellationToken cancellationToken = default) =>
            port.AddDeviceAsync(new() { Protocol = protocolDescriptor }, cancellationToken);
    }
}