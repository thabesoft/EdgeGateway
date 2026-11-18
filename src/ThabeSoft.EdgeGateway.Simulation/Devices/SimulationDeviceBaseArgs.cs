using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.Utilities;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Devices;

internal readonly struct SimulationDeviceBaseArgs
{
    public required SimulationDeviceDescriptor Device { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor DeviceStateMachine { get; init => field = value.Required(); }


    public required ILogger Logger { get; init => field = value.Required(); }
    public required TaskManager TaskManager { get; init => field = value.Required(); }
    public required TriggerManager TriggerManager { get; init => field = value.Required(); }
    public required ILifecycleStateMachineFactory LifecycleStateMachineFactory { get; init => field = value.Required(); }
}