namespace ThabeSoft.Lifecycles;


internal sealed class LifecycleFactory(IEnumerable<ILifecycleResolver> resolvers) : ILifecycleFactory
{
    public ILifecycle Create(LifecycleDescriptor descriptor)
    {
        var resolver = resolvers.FirstOrDefault(x => x.CanResolve(descriptor))
             ?? throw new InvalidOperationException($"不支持创建的生命周期({descriptor.GetType()}):{descriptor}");

        return resolver.Resolve(descriptor);
    }
}