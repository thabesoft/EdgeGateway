using EdgeGateway.Byte;

namespace EdgeGateway.Channel;


public interface IChannel : IChannelLifecycle, IByteStream, IAsyncDisposable
{
    ChannelAddress Address { get; }
}