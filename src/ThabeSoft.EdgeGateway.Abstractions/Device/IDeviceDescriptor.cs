using ThabeSoft.EdgeGateway.Channel;
using ThabeSoft.EdgeGateway.Protocol;

namespace ThabeSoft.EdgeGateway.Device;


public interface IDeviceDescriptor
{
    IProtocolDescriptor Protocol { get; }
    ConnectionType Connection { get; }
}

public sealed class DeviceDescriptor : IDeviceDescriptor
{
    public required IProtocolDescriptor Protocol { get; init; }
    public required ConnectionType Connection { get; init; }


    public static DeviceDescriptor Ethernet(IProtocolDescriptor protocolDescriptor)
    {
        ArgumentNullException.ThrowIfNull(protocolDescriptor);

        return new()
        {
            Connection = ConnectionType.Ethernet,
            Protocol = protocolDescriptor
        };
    }

    public static DeviceDescriptor Serial(IProtocolDescriptor protocolDescriptor)
    {
        ArgumentNullException.ThrowIfNull(protocolDescriptor);

        return new()
        {
            Connection = ConnectionType.Serial,
            Protocol = protocolDescriptor
        };
    }
}