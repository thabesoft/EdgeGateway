namespace ThabeSoft.Lifecycles.StateMachines;


public interface ILifecycleStateMachineFactory
{
    ILifecycleStateMachine Create(LifecycleStateMachineDescriptor descriptor);
}