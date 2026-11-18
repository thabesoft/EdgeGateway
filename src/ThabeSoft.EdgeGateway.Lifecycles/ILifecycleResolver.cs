namespace ThabeSoft.EdgeGateway.Lifecycles;

internal interface ILifecycleResolver
{
    bool CanResolve(LifecycleDescriptor descriptor);
    ILifecycle Resolve(LifecycleDescriptor descriptor);
}