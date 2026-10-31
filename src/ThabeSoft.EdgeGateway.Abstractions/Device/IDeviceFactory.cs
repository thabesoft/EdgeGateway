using ThabeSoft.EdgeGateway.Byte;

namespace ThabeSoft.EdgeGateway.Device;


public interface IDeviceFactory
{
    bool CanCreate(IDeviceDescriptor descriptor);
    IDevice Create(IDeviceDescriptor descriptor, IByteStream byteStream);
}