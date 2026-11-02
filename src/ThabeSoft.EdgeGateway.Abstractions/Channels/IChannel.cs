using ThabeSoft.EdgeGateway.IO;

namespace ThabeSoft.EdgeGateway.Channels;


public interface IChannel : IChannelLifecycle, IByteStream, IAsyncDisposable
{
    ChannelAddress Address { get; }
}