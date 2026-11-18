using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;

namespace ThabeSoft.EdgeGateway.Collections;


internal sealed class LifecycleBatchExecutorFactory(ILoggerFactory loggerFactory) : ILifecycleBatchExecutorFactory
{
    public ILifecycleBatchExecutor Create(LifecycleBatchExecutorDescriptor descriptor)
    {
        var logger = loggerFactory.CreateLogger<LifecycleBatchExecutor>();
        return new LifecycleBatchExecutor(descriptor, logger);
    }
}
