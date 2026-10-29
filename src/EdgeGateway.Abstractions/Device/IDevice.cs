namespace EdgeGateway.Device;


public interface IDevice : ITagReader, IDeviceLifecycle, IAsyncDisposable
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }
}

public interface IWritableDevice : IDevice, ITagWriter;