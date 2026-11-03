using ThabeSoft.EdgeGateway.Channels;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Options;

public interface IPortOptionsBuilder
{
    IPortOptionsBuilder SetDescriptor(IPortDescriptor descriptor);
    IPortOptionsBuilder AddDevice(Action<IDeviceOptionsBuilder> buildActnion);
}


public static class PortConfigBuilderExtensions
{
    extension(IPortOptionsBuilder builder)
    {
        public IPortOptionsBuilder SetDescriptor(IChannelDescriptor channelDescriptor)
        {
            ArgumentNullException.ThrowIfNull(channelDescriptor);
            return builder.SetDescriptor(PortDescriptor.Create(channelDescriptor));
        }
    }
}