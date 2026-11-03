using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Options;


public interface IDeviceOptionsBuilder
{
    IDeviceOptionsBuilder SetDescriptor(IDeviceDescriptor descriptor);
}

public static class DeviceOptionsBuilderExtensions
{
    extension(IDeviceOptionsBuilder builder)
    {
        public IDeviceOptionsBuilder UseProtocol(DeviceId deviceId, IProtocolDescriptor descriptor) =>
            builder.SetDescriptor(DeviceDescriptor.Create(deviceId, descriptor));

        public IDeviceOptionsBuilder UseProtocol(IProtocolDescriptor descriptor) =>
            builder.SetDescriptor(DeviceDescriptor.Create(descriptor));
    }
}