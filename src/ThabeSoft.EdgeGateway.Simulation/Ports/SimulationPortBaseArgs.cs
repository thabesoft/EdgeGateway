using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Ports;

internal readonly struct SimulationPortBaseArgs
{
    public required TransportDescriptor Transport { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor PortStateMachine { get; init => field = value.Required(); }


    public required SimulationDeviceFactory DeviceFactory { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor DeviceBatchExecutor { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor DeviceStateMachine { get; init => field = value.Required(); }


    public required ILifecycleBatchExecutorFactory LifecycleBatchExecutorFactory { get; init => field = value.Required(); }
    public required ILifecycleStateMachineFactory LifecycleStateMachineFactory { get; init => field = value.Required(); }
    public required ILifecycleKeyedCollectionFactory LifecycleKeyedCollectionFactory { get; init => field = value.Required(); }
    public required ILogger Logger { get; init => field = value.Required(); }
}