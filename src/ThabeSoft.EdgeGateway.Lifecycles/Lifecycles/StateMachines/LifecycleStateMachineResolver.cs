namespace ThabeSoft.EdgeGateway.Lifecycles.StateMachines;


internal sealed class LifecycleStateMachineResolver(
        IEnumerable<ILifecycleStateMachineResolver> resolvers
    ) : ILifecycleStateMachineFactory
{
    public ILifecycleStateMachine Create(LifecycleStateMachineDescriptor descriptor)
    {
        var resolver = resolvers.FirstOrDefault(x => x.CanCreate(descriptor))
             ?? throw new InvalidOperationException($"不支持创建的生命周期状态机({descriptor.GetType()}):{descriptor}");

        return resolver.Resolve(descriptor);
    }
}