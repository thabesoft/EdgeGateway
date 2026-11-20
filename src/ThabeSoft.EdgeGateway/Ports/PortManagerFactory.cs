using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.Primitives;

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
                await Safe.DisposeSafelyAsync(batchExecutor, logger);
                await Safe.DisposeSafelyAsync(collection, logger);
            }
        };
    }
}