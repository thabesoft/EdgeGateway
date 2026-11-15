using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Protocols;

public abstract record ProtocolDescriptor : IEquatable<ProtocolDescriptor>
{
    public abstract ProtocolType Type { get; }
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