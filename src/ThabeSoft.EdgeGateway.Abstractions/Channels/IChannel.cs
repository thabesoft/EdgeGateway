using ThabeSoft.EdgeGateway.IO;

namespace ThabeSoft.EdgeGateway.Channels;


public interface IChannel : IChannelLifecycle, IByteStream, IAsyncDisposable
{
    ChannelId Id { get; }
    ChannelAddress Address { get; }
}