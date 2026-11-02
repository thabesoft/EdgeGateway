namespace ThabeSoft.EdgeGateway.Channels;


public interface IChannelFactory
{
    bool CanCreate(IChannelDescriptor descriptor);
    IChannel Create(IChannelDescriptor descriptor);
}