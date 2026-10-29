using EdgeGateway.Byte;

namespace EdgeGateway.Channel;


public interface IChannel : IChannelLifecycle, IByteStream, IAsyncDisposable
{
    ChannelAddress Address { get; }
}

public interface IChannelManager
{
    IReadOnlyDictionary<ChannelAddress, IChannel> Channels { get; }


    IChannel? FindChannel(ChannelAddress address);
    IChannel AddChannel(IChannelDescriptor descriptor);
    ValueTask RemoveChannelAsync(ChannelAddress address, CancellationToken cancellationToken = default);
}