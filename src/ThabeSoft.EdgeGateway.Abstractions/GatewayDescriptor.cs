using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway;


public sealed class GatewayDescriptor
{
    public LifecycleStateMachineDescriptor StateMachine { get; init => field = value.Required(); } = Lifecycle.RetryableStateMachine(x => x
        .WithStartTimeout(TimeSpan.FromMinutes(5))
        .WithStopTimeout(TimeSpan.FromMinutes(3))
        .WithDisposeTimeout(TimeSpan.FromMinutes(1))
        .WithExecuteTimeout(TimeSpan.FromSeconds(10))
    );
    public LifecycleBatchExecutorDescriptor PortBatchExecutor { get; init => field = value.Required(); } = Lifecycle.BatchExecutor(x => x
        .WithItemStartTimeout(TimeSpan.FromSeconds(30))
        .WithItemStopTimeout(TimeSpan.FromSeconds(30))
        .WithItemDisposeTimeout(TimeSpan.FromSeconds(10))
        .WithMaxConcurrency(3)
    );
}