using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Transports;

namespace ThabeSoft.EdgeGateway.Ports;


public sealed record class PortDescriptor
{
    public PortId Id { get; init => field = value.Required(); } = PortId.Create();
    public required TransportDescriptor Transport { get; init => field = value.Required(); }


    public LifecycleStateMachineDescriptor StateMachine { get; init => field = value.Required(); } = LifecycleBuilder.RetryableStateMachine(x => x
        .WithStartTimeout(TimeSpan.FromSeconds(30))
        .WithStopTimeout(TimeSpan.FromSeconds(15))
        .WithDisposeTimeout(TimeSpan.FromSeconds(10))
        .WithExecuteTimeout(TimeSpan.FromSeconds(5))
    );
    public LifecycleBatchExecutorDescriptor DeviceBatchExecutor { get; init => field = value.Required(); } = LifecycleBuilder.BatchExecutor(x => x
        .WithItemStartTimeout(TimeSpan.FromSeconds(5))
        .WithItemStopTimeout(TimeSpan.FromSeconds(5))
        .WithItemDisposeTimeout(TimeSpan.FromSeconds(5))
        .WithMaxConcurrency(3)
    );
}