using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Subscription;


internal readonly struct SubscriptionCreationArgs<T> where T : notnull
{
    // Port
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required IProtocolAccessor ProtocolAccessor { get; init => field = value.Required(); }

    // Device
    public required DeviceDescriptor Device { get; init => field = value.Required(); }
    public required ILifecycleStateable DeviceState { get; init => field = value.Required(); }

    // Subscription
    public required SubscriptionDescriptor<T> Subscription { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor SubscriptioStateMachineDescriptor { get; init => field = value.Required(); }
}