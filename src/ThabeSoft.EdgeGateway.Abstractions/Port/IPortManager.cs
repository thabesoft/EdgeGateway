using EdgeGateway.Channel;

namespace EdgeGateway.Port;


public interface IPortManager
{
    IReadOnlyDictionary<PortId, IPort> Ports { get; }

    IPort AddPort(IPortDescriptor descriptor);
    ValueTask RemovePortAsync(PortId id, CancellationToken cancellationToken = default);
}


public static class PortManagerExtensions
{
    extension(IPortManager portManager)
    {
        public IPort AddPort(IChannelDescriptor channelDescriptor)
        {
            ArgumentNullException.ThrowIfNull(channelDescriptor);
            return portManager.AddPort(PortDescriptor.From(channelDescriptor));
        }
    }
}