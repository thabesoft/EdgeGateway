using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Devices;


public sealed record class DeviceDescriptor
{
    public DeviceId Id { get; init => field = value.Required(); } = DeviceId.Create();
    public required ProtocolDescriptor Protocol { get; init => field = value.Required(); }
    public LifecycleStateMachineDescriptor StateMachine { get; init => field = value.Required(); } = LifecycleBuilder.RetryableStateMachine();
    public LifecycleBatchExecutorDescriptor SubscriptionBatchExecutor { get; init; } = LifecycleBuilder.BatchExecutor();
}