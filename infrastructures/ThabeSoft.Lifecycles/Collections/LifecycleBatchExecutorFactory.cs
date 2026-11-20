using Microsoft.Extensions.Logging;

namespace ThabeSoft.Lifecycles.Collections;


internal sealed class LifecycleBatchExecutorFactory(ILoggerFactory loggerFactory) : ILifecycleBatchExecutorFactory
{
    public ILifecycleBatchExecutor Create(LifecycleBatchExecutorDescriptor descriptor)
    {
        var logger = loggerFactory.CreateLogger<LifecycleBatchExecutor>();
        return new LifecycleBatchExecutor(descriptor, logger);
    }
}
