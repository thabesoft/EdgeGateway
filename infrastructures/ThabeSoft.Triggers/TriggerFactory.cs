namespace ThabeSoft.Triggers;


internal sealed class TriggerFactory(
        IEnumerable<ITriggerResolver> resolvers
    ) : ITriggerFactory
{
    public ITrigger Create(TriggerDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);

        var factory = resolvers.FirstOrDefault(x => x.CanResolve(descriptor))
            ?? throw new InvalidOperationException($"不支持创建的触发器({descriptor.GetType()}):{descriptor}");

        return factory.Resolve(descriptor);
    }
}