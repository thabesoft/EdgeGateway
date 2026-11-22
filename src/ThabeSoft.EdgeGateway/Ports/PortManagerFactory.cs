using Microsoft.Extensions.Logging;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.PubSub;

namespace ThabeSoft.EdgeGateway.Ports;


internal sealed class PortManagerFactory(
        IEventPublisher eventPublisher,
        ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory,
        PortFactory portFactory,
        ILoggerFactory loggerFactory
    )
{
    public PortManager Create(PortManagerCreationArgs args)
    {
        var logger = loggerFactory.CreateLogger<PortManager>();
        var collection = lifecycleKeyedCollectionFactory.Create<PortId, IPort>(args.GatwayLifecycleState);
        var batchExecutor = lifecycleBatchExecutorFactory.Create(args.PortBatchExecutorDescriptor);

        // 端口管理依赖
        var portManagerDependencies = new PortManagerDependencies
        {
            Collection = collection,
            BatchExecutor = batchExecutor,

            EventPublisher =eventPublisher,
            Logger = logger,
            PortCreator = port =>
            {
                var portCreationArgs = new PortCreationArgs()
                {
                    GatewayLifecycleState = args.GatwayLifecycleState,
                    Port = port
                };

                return portFactory.Create(portCreationArgs);
            },
            OnDispose = async () =>
            {
                await Safe.DisposeSafelyAsync(batchExecutor, logger);
                await Safe.DisposeSafelyAsync(collection, logger);
            }
        };

        return new(portManagerDependencies);
    }
}


internal readonly struct PortManagerCreationArgs
{
    public required ILifecycleStateable GatwayLifecycleState { get; init => field = value.Required(); }
    public required Func<PortDescriptor, Port> PortCreator { get; init => field = value.Required(); }
    public required LifecycleBatchExecutorDescriptor PortBatchExecutorDescriptor { get; init => field = value.Required(); }
}