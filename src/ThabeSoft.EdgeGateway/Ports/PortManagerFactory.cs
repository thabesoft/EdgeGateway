using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortManagerFactory(
        IEventPublisher eventPublisher,
        ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory,
        ILoggerFactory loggerFactory
    )
{
    public PortManager Create(in PortManagerCreationArgs args)
    {
        var collection = lifecycleKeyedCollectionFactory.Create<PortId, IPort>(args.GatwayLifecycleState);
        var batchExecutor = lifecycleBatchExecutorFactory.Create(args.PortBatchExecutorDescriptor);
        var logger = loggerFactory.CreateLogger<PortManager>();


        return new
        (
            collection: collection,
            batchExecutor: batchExecutor,
            portCreator: args.PortCreator,
            eventPublisher: eventPublisher,
            logger: logger
        )
        {
            DisposedCallback = async () =>
            {
                await SafeDisposeExtensions.DisposeSafelyAsync(batchExecutor, logger);
                await SafeDisposeExtensions.DisposeSafelyAsync(collection, logger);
            }
        };
    }
}