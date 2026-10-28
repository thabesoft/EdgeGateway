namespace EdgeGateway.Device;


public interface IDevice : IDeviceLifecycle
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }
}
