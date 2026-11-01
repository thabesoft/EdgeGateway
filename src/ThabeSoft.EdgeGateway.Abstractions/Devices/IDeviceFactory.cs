using ThabeSoft.EdgeGateway.IO;

namespace ThabeSoft.EdgeGateway.Devices;


public interface IDeviceFactory
{
    bool CanCreate(IDeviceDescriptor descriptor);
    IDevice Create(IDeviceDescriptor descriptor, IByteStream byteStream);
}