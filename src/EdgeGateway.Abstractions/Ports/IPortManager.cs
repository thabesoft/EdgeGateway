using EdgeGateway.Bytes;

namespace EdgeGateway.Ports;


public interface IPortManager
{
    IReadOnlyDictionary<IChannelAddress, IPort> Ports { get; }


    IPort Add(ChannelDescriptor descriptor);
    ValueTask RemoveAsync(IChannelAddress address, CancellationToken cancellationToken = default);
}