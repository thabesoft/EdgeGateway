using EdgeGateway.Byte;

namespace EdgeGateway.Channel;


public interface IChannel : IByteStream, IAsyncDisposable
{
    ChannelAddress Address { get; }

    ValueTask OpenAsync(CancellationToken cancellationToken = default);
    ValueTask CloseAsync(CancellationToken cancellationToken = default);
}