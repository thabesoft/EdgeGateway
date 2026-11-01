using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Devices;


public interface IDeviceDescriptor
{
    DeviceId Id { get; }
    IProtocolDescriptor Protocol { get; }
}

public sealed class DeviceDescriptor : IDeviceDescriptor
{
    public required DeviceId Id { get; init; }
    public required IProtocolDescriptor Protocol { get; init; }


    internal DeviceDescriptor() { }
    public static DeviceDescriptor Create(DeviceId deviceId, IProtocolDescriptor protocolDescriptor)
    {
        ArgumentNullException.ThrowIfNull(deviceId);
        ArgumentNullException.ThrowIfNull(protocolDescriptor);

        return new() { Id = deviceId, Protocol = protocolDescriptor };
    }
    public static DeviceDescriptor Create(IProtocolDescriptor protocolDescriptor) =>
        Create(DeviceId.Create(), protocolDescriptor);
}