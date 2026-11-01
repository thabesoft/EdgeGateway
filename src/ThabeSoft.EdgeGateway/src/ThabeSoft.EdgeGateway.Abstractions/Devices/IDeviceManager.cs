using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Devices;

public interface IDeviceManager
{
    IReadOnlyDictionary<DeviceId, IDevice> Devices { get; }

    IDevice AddDevice(IDeviceDescriptor descriptor);
    ValueTask RemoveDeviceAsync(DeviceId id, CancellationToken cancellationToken = default);
}


public static class DeviceManagerExtensions
{
    extension(IDeviceManager deviceManager)
    {
        public IDevice AddDevice(DeviceId deviceId, IProtocolDescriptor protocolDescriptor)
        {
            ArgumentNullException.ThrowIfNull(deviceId);
            ArgumentNullException.ThrowIfNull(protocolDescriptor);


            return deviceManager.AddDevice(DeviceDescriptor.Create(deviceId, protocolDescriptor));
        }

        public IDevice AddDevice(IProtocolDescriptor protocolDescriptor) =>
            deviceManager.AddDevice(DeviceId.Create(), protocolDescriptor);
    }
}