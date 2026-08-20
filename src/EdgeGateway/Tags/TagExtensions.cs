using EdgeGateway.Data;
using EdgeGateway.Devices;
using EdgeGateway.Ports;

namespace EdgeGateway.Tags;


public static class TagExtensions
{
    extension(Tag)
    {
        public static Tag<T> Create<T>(PortAddress portAddress, DeviceId deviceId, IDataAddress dataAddress)
            where T : unmanaged
        {
            return new()
            {
                PortAddress = portAddress,
                DeviceId = deviceId,
                DataDescriptor = DataDescriptor.Create(dataAddress, TypeDescriptors.Unmanaged<T>())
            };
        }

        public static Tag<DynamicType> CreateDyanmic(PortAddress portAddress, DeviceId deviceId, IDataAddress dataAddress)
        {
            return new()
            {
                PortAddress = portAddress,
                DeviceId = deviceId,
                DataDescriptor = DataDescriptor.Create(dataAddress, TypeDescriptors.Dyanmic())
            };
        }
    }
}