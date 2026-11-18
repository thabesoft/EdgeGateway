using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class SimulationPortFactory(IEnumerable<ISimulationPortResolver> resolvers)
{
    public ISimulationPort Create(SimulatorPortCreationContext context)
    {
        var factory = resolvers.FirstOrDefault(x => x.CanCreate(context))
            ?? throw new InvalidOperationException("没有可创建的工厂");

        return factory.Resolve(context);
    }
}


public sealed class SimulatorPortCreationContext
{
    public required SimulatorPortDescriptor Port { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor PortStateMachine { get; init => field = value.Required(); }

    public required LifecycleBatchExecutorDescriptor DeviceBatchExecutor { get; init => field = value.Required(); }
    public required LifecycleStateMachineDescriptor DeviceStateMachine { get; init => field = value.Required(); }
}