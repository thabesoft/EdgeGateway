namespace ThabeSoft.Lifecycles;

internal interface ILifecycleResolver
{
    bool CanResolve(LifecycleDescriptor descriptor);
    ILifecycle Resolve(LifecycleDescriptor descriptor);
}