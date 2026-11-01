using ThabeSoft.EdgeGateway.Channels;

namespace ThabeSoft.EdgeGateway.Ports;


public interface IPortDescriptor
{
    IChannelDescriptor ChannelDescriptor { get; }
}

public sealed record class PortDescriptor : IPortDescriptor
{
    public required IChannelDescriptor ChannelDescriptor { get; init; }


    private PortDescriptor() { }
    public static PortDescriptor From(IChannelDescriptor channelDescriptor)
    {
        ArgumentNullException.ThrowIfNull(channelDescriptor);
        return new() { ChannelDescriptor = channelDescriptor };
    }
}