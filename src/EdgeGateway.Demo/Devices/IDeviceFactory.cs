using EdgeGateway.Channels;

namespace EdgeGateway.Devices;


public interface IDeviceFactory
{
    bool CanCreate(IDeviceDescriptor descriptor);
    IDevice Create(IDeviceDescriptor descriptor, IByteChannel byteChannel);
}