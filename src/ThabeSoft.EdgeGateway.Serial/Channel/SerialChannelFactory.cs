using ThabeSoft.EdgeGateway.Channel;

namespace ThabeSoft.EdgeGateway.Serial.Channel;


internal sealed class SerialChannelFactory : IChannelFactory
{
    public bool CanCreate(IChannelDescriptor descriptor)
    {
        return descriptor is SerialChannelDescriptor;
    }

    public IChannel Create(IChannelDescriptor descriptor)
    {
        if (descriptor is not SerialChannelDescriptor serial)
            throw new InvalidOperationException();

        return new SerialChannel(serial);
    }
}