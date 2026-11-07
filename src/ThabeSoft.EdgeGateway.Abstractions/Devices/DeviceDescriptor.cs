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