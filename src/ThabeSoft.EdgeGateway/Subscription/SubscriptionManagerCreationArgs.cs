using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Subscription;


internal readonly struct SubscriptionManagerCreationArgs
{
    public required PortDescriptor Port { get; init => field = value.Required(); }

    public required DeviceDescriptor Device { get; init => field = value.Required(); }
    public required ILifecycleStateable DeviceState { get; init => field = value.Required(); }

    public required ISubscriptionCreator SubscriptionCreator { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor SubscriptionBatchExecutor { get; init => field = value.Required(); }
}