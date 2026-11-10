using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Devices;


public sealed class DeviceDescriptor
{
    public required DeviceId Id { get; init; }
    public required IProtocolDescriptor Protocol { get; init; }


    private DeviceDescriptor() { }
    public override string ToString() => $"{Id} @ {Protocol}";


    public static DeviceDescriptor Create(DeviceId deviceId, IProtocolDescriptor protocolDescriptor)
    {
        ArgumentNullException.ThrowIfNull(deviceId);
        ArgumentNullException.ThrowIfNull(protocolDescriptor);

        return new() { Id = deviceId, Protocol = protocolDescriptor };
    }
    public static DeviceDescriptor Create(IProtocolDescriptor protocolDescriptor) =>
        Create(DeviceId.Create(), protocolDescriptor);
}


public static class PortExtensions
{
    extension(IPort port)
    {
        public ValueTask<IDevice> AddDeviceAsync(DeviceId deviceId, IProtocolDescriptor protocolDescriptor, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(deviceId);
            ArgumentNullException.ThrowIfNull(protocolDescriptor);


            return port.AddDeviceAsync(DeviceDescriptor.Create(deviceId, protocolDescriptor), cancellationToken);
        }

        public ValueTask<IDevice> AddDeviceAsync(IProtocolDescriptor protocolDescriptor, CancellationToken cancellationToken = default) =>
            port.AddDeviceAsync(DeviceId.Create(), protocolDescriptor, cancellationToken);
    }
}