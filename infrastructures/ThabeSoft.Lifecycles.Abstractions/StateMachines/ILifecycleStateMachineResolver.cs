namespace ThabeSoft.Lifecycles.StateMachines;


public interface ILifecycleStateMachineResolver
{
    bool CanResolve(LifecycleStateMachineDescriptor descriptor);
    ILifecycleStateMachine Resolve(LifecycleStateMachineDescriptor descriptor);
}
