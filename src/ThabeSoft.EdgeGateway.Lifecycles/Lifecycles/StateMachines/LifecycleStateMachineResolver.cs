namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;


internal sealed class LifecycleStateMachineResolver(
        IEnumerable<ILifecycleStateMachineResolver> factories
    ) : ILifecycleStateMachineFactory
{
    public ILifecycleStateMachine Create(LifecycleStateMachineDescriptor descriptor)
    {
        var factory = factories.FirstOrDefault(x => x.CanCreate(descriptor))
            ?? throw new InvalidOperationException($"不支持的生命状态机描述符: {descriptor}");

        return factory.Create(descriptor);
    }
}