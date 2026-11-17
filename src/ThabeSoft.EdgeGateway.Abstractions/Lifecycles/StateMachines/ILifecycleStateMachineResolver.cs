namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;


public interface ILifecycleStateMachineResolver
{
    bool CanCreate(LifecycleStateMachineDescriptor descriptor);
    ILifecycleStateMachine Resolve(LifecycleStateMachineDescriptor descriptor);
}
