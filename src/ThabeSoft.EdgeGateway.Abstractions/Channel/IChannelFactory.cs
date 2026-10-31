namespace EdgeGateway.Channel;


public interface IChannelFactory
{
    bool CanCreate(IChannelDescriptor descriptor);
    IChannel Create(IChannelDescriptor descriptor);
}