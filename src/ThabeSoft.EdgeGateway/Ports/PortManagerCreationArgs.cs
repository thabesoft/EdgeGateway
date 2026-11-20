using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Ports;

internal readonly struct PortManagerCreationArgs
{
    public required ILifecycleStateable GatwayLifecycleState { get; init => field = value.Required(); }
    public required Func<PortDescriptor, Port> PortCreator { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor PortBatchExecutorDescriptor { get; init => field = value.Required(); }
}