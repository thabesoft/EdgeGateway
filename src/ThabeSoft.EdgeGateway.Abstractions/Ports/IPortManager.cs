using ThabeSoft.EdgeGateway.Transport;

namespace ThabeSoft.EdgeGateway.Ports;

public interface IPortManager
{
    IReadOnlyDictionary<ConnectionEndpoint, IPort> Ports { get; }

    IPort AddPort(IPortDescriptor descriptor);
    ValueTask RemovePortAsync(ConnectionEndpoint id, CancellationToken cancellationToken = default);
}

public static class PortManagerExtensions
{
    extension(IPortManager portManager)
    {
        public IPort AddPort(ITransportDescriptor channelDescriptor)
        {
            ArgumentNullException.ThrowIfNull(channelDescriptor);

            return portManager.AddPort(PortDescriptor.Create(channelDescriptor));
        }
    }
}