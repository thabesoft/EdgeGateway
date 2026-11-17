using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Devices;

internal readonly struct DeviceManagerCreationArgs
{
    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required ILifecycleStateable PortLifecycleState { get; init => field = value.Required(); }

    // Device
    public required Func<DeviceDescriptor, Device> DevicCreator { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor DeviceBatchExecutorDescriptor { get; init => field = value.Required(); }
}