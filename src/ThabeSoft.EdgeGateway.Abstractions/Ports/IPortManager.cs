using ThabeSoft.EdgeGateway.Channels;

namespace ThabeSoft.EdgeGateway.Ports;

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
        public IPort AddPort(PortId portId, IChannelDescriptor channelDescriptor)
        {
            ArgumentNullException.ThrowIfNull(portId);
            ArgumentNullException.ThrowIfNull(channelDescriptor);

            return portManager.AddPort(PortDescriptor.Create(portId, channelDescriptor));
        }

        public IPort AddPort(IChannelDescriptor channelDescriptor)
        {
            ArgumentNullException.ThrowIfNull(channelDescriptor);

            return portManager.AddPort(PortDescriptor.Create(channelDescriptor));
        }
    }
}