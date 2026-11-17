namespace ThabeSoft.EdgeGateway.Recoveries;


internal sealed class RecoveryPolicyFactoryResolver(IEnumerable<IRecoveryPolicyFactory> factories)
{
    public IRecoveryPolicy Resolve(IRecoveryPolicyDescriptor descriptor)
    {
        var factory = factories.FirstOrDefault(x => x.CanCreate(descriptor))
            ?? throw new InvalidOperationException("不支持的重试策略描述符");

        return factory.Create(descriptor);
    }
}