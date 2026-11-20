using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Protocols;

namespace ThabeSoft.EdgeGateway.Devices;


internal readonly struct DeviceCreationArgs
{
    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required ILifecycleStateable PortState { get; init; }
    public required IProtocolAccessor ProtocolAccessor { get; init => field = value.Required(); }

    // Device
    public required DeviceDescriptor Device { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor DeviceStateMachineDescriptor { get; init; }


    // Subscription
    public required LifecycleStateMachineDescriptor SubscriptionStateMachineDescriptor { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor SubscriptionBatchExecutorDescriptor { get; init; }
}