using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.Lifecycles;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Primitives;
using ThabeSoft.Primitives.Guard;
using ThabeSoft.Protocols.Accessor;
using ThabeSoft.PubSub;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionManagerFactory(
        IEventPublisher eventPublisher,
        ILifecycleBatchExecutorFactory lifecycleBatchExecutorFactory,
        ILifecycleKeyedCollectionFactory lifecycleKeyedCollectionFactory,
        SubscriptionFactory subscriptionFactory,
        ILoggerFactory loggerFactory
    )
{
    public SubscriptionManager Create(in SubscriptionManagerCreationArgs args)
    {
        var logger = loggerFactory.CreateLogger<SubscriptionManager>();
        var collection = lifecycleKeyedCollectionFactory.Create<SubscriptionId, ISubscription>(args.DeviceState);
        var batchExecutor = lifecycleBatchExecutorFactory.Create(args.Device.SubscriptionBatchExecutor);


        var subscriptionManagerDependencies = new SubscriptionManagerDependencies
        {
            Port = args.Port,
            Device = args.Device,

            Collection = collection,
            BatchExecutor = batchExecutor,
            SubscriptionCreator = new SubscriptionCreator(args, subscriptionFactory),

            Logger = logger,
            EventPublisher = eventPublisher,
            OnDispose = async () =>
            {
                await Safe.DisposeSafelyAsync(batchExecutor, logger);
                await Safe.DisposeSafelyAsync(collection, logger);
            }
        };

        return new(subscriptionManagerDependencies);
    }

    private sealed class SubscriptionCreator(SubscriptionManagerCreationArgs args, SubscriptionFactory factory) : ISubscriptionCreator
    {
        public ISubscription<T> Create<T>(SubscriptionDescriptor<T> descriptor) where T : notnull
        {
            var creationArgs = new SubscriptionCreationArgs<T>
            {
                Port = args.Port,
                ProtocolAccessor = args.ProtocolAccessor,

                Device = args.Device,
                DeviceState = args.DeviceState,

                Subscription = descriptor
            };

            return factory.Create(creationArgs);
        }
    }
}

internal readonly struct SubscriptionManagerCreationArgs
{
    public required PortDescriptor Port { get; init => field = value.Required(); }
    public required IProtocolAccessor ProtocolAccessor { get; init => field = value.Required(); }


    public required DeviceDescriptor Device { get; init => field = value.Required(); }
    public required ILifecycleStateable DeviceState { get; init => field = value.Required(); }

    public required SubscriptionFactory SubscriptionFactory { get; init => field = value.Required(); }
}