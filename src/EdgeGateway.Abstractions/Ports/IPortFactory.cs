using EdgeGateway.Bytes;

namespace EdgeGateway.Ports;


public interface IPortFactory
{
    bool CanCreate(ChannelDescriptor descriptor);
    IPort Create(ChannelDescriptor descriptor);
}