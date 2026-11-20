namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;


public interface ILifecycleStateMachineFactory
{
    ILifecycleStateMachine Create(LifecycleStateMachineDescriptor descriptor);
}