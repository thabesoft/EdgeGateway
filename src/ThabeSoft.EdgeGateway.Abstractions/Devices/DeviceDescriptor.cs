using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Devices;


public sealed class DeviceDescriptor
{
    public required DeviceId DeviceId { get; init; }
    public required IProtocolDescriptor Protocol { get; init; }


    internal DeviceDescriptor() { }
    public static DeviceDescriptor Create(DeviceId deviceId, IProtocolDescriptor protocolDescriptor)
    {
        ArgumentNullException.ThrowIfNull(deviceId);
        ArgumentNullException.ThrowIfNull(protocolDescriptor);

        return new() { DeviceId = deviceId, Protocol = protocolDescriptor };
    }
    public static DeviceDescriptor Create(IProtocolDescriptor protocolDescriptor) =>
        Create(DeviceId.Create(), protocolDescriptor);
}