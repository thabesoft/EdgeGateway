namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;


public interface ILifecycleStateMachineResolver
{
    bool CanCreate(LifecycleStateMachineDescriptor descriptor);
    ILifecycleStateMachine Create(LifecycleStateMachineDescriptor descriptor);
}
