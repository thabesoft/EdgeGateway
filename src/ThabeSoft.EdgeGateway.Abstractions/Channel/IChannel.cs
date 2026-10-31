using ThabeSoft.EdgeGateway.Byte;

namespace ThabeSoft.EdgeGateway.Channel;


public interface IChannel : IChannelLifecycle, IByteStream, IAsyncDisposable
{
    ChannelAddress Address { get; }
}