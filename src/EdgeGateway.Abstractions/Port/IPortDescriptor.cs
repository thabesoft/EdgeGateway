using EdgeGateway.Channel;

namespace EdgeGateway.Port;

public interface IPortDescriptor;


public sealed record class ChannelPortDescriptor : IPortDescriptor
{
    public required IChannelDescriptor ChannelDescriptor { get; init; }


    private ChannelPortDescriptor() { }
    public static ChannelPortDescriptor From(IChannelDescriptor channelDescriptor)
    {
        ArgumentNullException.ThrowIfNull(channelDescriptor);
        return new() { ChannelDescriptor = channelDescriptor };
    }
}