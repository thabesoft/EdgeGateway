using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Devices;


public sealed class DeviceDescriptor : IEquatable<DeviceDescriptor>
{
    public DeviceId Id { get; }
    public IProtocolDescriptor Protocol { get; }


    public DeviceDescriptor(DeviceId deviceId, IProtocolDescriptor protocolDescriptor)
    {
        ArgumentNullException.ThrowIfNull(deviceId);
        ArgumentNullException.ThrowIfNull(protocolDescriptor);

        Id = deviceId;
        Protocol = protocolDescriptor;
    }
    public DeviceDescriptor(IProtocolDescriptor protocolDescriptor) : this(DeviceId.Create(), protocolDescriptor)
    {

    }

    public bool Equals(DeviceDescriptor other)
    {
        return Id.Equals(other.Id) && Protocol.Equals(other.Protocol);
    }
    public override string ToString() => $"{Id} @ {Protocol}";
}


public static class PortExtensions
{
    extension(IPort port)
    {
        public ValueTask<IDevice> AddDeviceAsync(DeviceId deviceId, IProtocolDescriptor protocolDescriptor, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(deviceId);
            ArgumentNullException.ThrowIfNull(protocolDescriptor);


            return port.AddDeviceAsync(new(deviceId, protocolDescriptor), cancellationToken);
        }

        public ValueTask<IDevice> AddDeviceAsync(IProtocolDescriptor protocolDescriptor, CancellationToken cancellationToken = default) =>
            port.AddDeviceAsync(new(protocolDescriptor), cancellationToken);
    }
}