using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Devices;


internal sealed class SimulationDeviceFactory(IEnumerable<ISimulationDeviceResolver> factories)
{
    public ISimulationDevice Create(SimulationDeviceCreationContext context)
    {
        var factory = factories.FirstOrDefault(x => x.CanResolve(context))
            ?? throw new InvalidOperationException("没有可创建的工厂");

        return factory.Resolve(context);
    }
}

internal sealed class SimulationDeviceCreationContext
{
    public required SimulationDeviceDescriptor Device { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor DeviceStateMachine { get; init => field = value.Required(); }
}