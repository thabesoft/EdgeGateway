using ThabeSoft.EdgeGateway.Channels;

namespace ThabeSoft.EdgeGateway.Ports;


public interface IPortDescriptor
{
    PortId PortId { get; }
    IChannelDescriptor ChannelDescriptor { get; }
}

public sealed record class PortDescriptor : IPortDescriptor
{
    public required PortId PortId { get; init; }
    public required IChannelDescriptor ChannelDescriptor { get; init; }


    private PortDescriptor() { }
    public static PortDescriptor Create(PortId portId, IChannelDescriptor channelDescriptor)
    {
        ArgumentNullException.ThrowIfNull(portId);
        ArgumentNullException.ThrowIfNull(channelDescriptor);

        return new()
        {
            PortId = portId,
            ChannelDescriptor = channelDescriptor
        };
    }
    public static PortDescriptor Create(IChannelDescriptor channelDescriptor) =>
        Create(PortId.Create(), channelDescriptor);
}