using EdgeGateway.Tag;

namespace EdgeGateway.Device;


public interface IDevice : IDeviceLifecycle
{
    DeviceId Id { get; }
    ProtocolType ProtocolType { get; }
}

public interface ITagDevice : IDevice, ITagReader, ITagWriter;