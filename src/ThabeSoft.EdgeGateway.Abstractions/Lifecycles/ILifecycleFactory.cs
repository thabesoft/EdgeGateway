using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Lifecycles;


public interface ILifecycleFactory
{
    ILifecycle Create(LifecycleDescriptor descriptor);
}



public abstract record class LifecycleDescriptor;


public sealed record class BackgroundTaskLifecycleDescriptor : LifecycleDescriptor
{
    public required Func<CancellationToken, Task> Task { get; init => field = value.Required(); }
}