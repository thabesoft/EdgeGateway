
using Microsoft.Extensions.Logging;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Lifecycles.BackgroundTask;


internal sealed class BakgroundTaskLifecycleResolver(ILoggerFactory loggerFactory) : ILifecycleResolver
{
    public bool CanResolve(LifecycleDescriptor descriptor)
    {
        return descriptor is BackgroundTaskLifecycleDescriptor;
    }

    public ILifecycle Resolve(LifecycleDescriptor descriptor)
    {
        var backgroundTask = descriptor.AssertType<BackgroundTaskLifecycleDescriptor>();
        var logger = loggerFactory.CreateLogger<BackgroundLifecycle>();

        return new BackgroundLifecycle(backgroundTask.Task, logger);
    }
}
