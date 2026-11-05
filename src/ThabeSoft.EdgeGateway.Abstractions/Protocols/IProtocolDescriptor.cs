using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;

namespace ThabeSoft.EdgeGateway.Protocols;

public interface IProtocolDescriptor
{
    ProtocolType Type { get; }
}

public static class PortExtensions
{
    extension(IPort port)
    {
        public ValueTask<IDevice> AddDeviceAsync(DeviceId deviceId, IProtocolDescriptor protocolDescriptor)
        {
            ArgumentNullException.ThrowIfNull(deviceId);
            ArgumentNullException.ThrowIfNull(protocolDescriptor);


            return port.AddDeviceAsync(DeviceDescriptor.Create(deviceId, protocolDescriptor));
        }

        public ValueTask<IDevice> AddDeviceAsync(IProtocolDescriptor protocolDescriptor) =>
            port.AddDeviceAsync(DeviceId.Create(), protocolDescriptor);
    }
}