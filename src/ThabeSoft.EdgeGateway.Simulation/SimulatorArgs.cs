using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Primitives.Guard;

namespace ThabeSoft.EdgeGateway;

internal readonly struct SimulatorArgs
{
    public required LifecycleStateMachineDescriptor SimulatorStateMachine { get; init => field = value.Required(); }

    public required SimulationPortFactory PortFactory { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor PortBatchExecutor { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor PortStateMachine { get; init => field = value.Required(); }


    public required LifecycleBatchExecutorDescriptor DeviceBatchExecutor { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor DeviceStateMachine { get; init => field = value.Required(); }


    public required ILifecycleBatchExecutorFactory LifecycleBatchExecutorFactory { get; init => field = value.Required(); }
    public required ILifecycleStateMachineFactory LifecycleStateMachineFactory { get; init => field = value.Required(); }
    public required ILogger Logger { get; init => field = value.Required(); }
}