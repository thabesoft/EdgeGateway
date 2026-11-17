//using Microsoft.Extensions.Logging;
//using System.Collections.Concurrent;
//using ThabeSoft.EdgeGateway.Devices;
//using ThabeSoft.EdgeGateway.Events;
//using ThabeSoft.EdgeGateway.Lifecycles;
//using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
//using ThabeSoft.EdgeGateway.Ports;
//using ThabeSoft.EdgeGateway.Protocols;
//using ThabeSoft.EdgeGateway.Readings;
//using ThabeSoft.EdgeGateway.Subscription;
//using ThabeSoft.EdgeGateway.Transports;
//using ThabeSoft.EdgeGateway.Triggers;
//using ThabeSoft.EdgeGateway.Utilities;
//using ThabeSoft.Guard;

//namespace ThabeSoft.EdgeGateway;


//internal sealed class GatewayFactory(
//    ITriggerScheduler triggerScheduler,
//    ProtocolAccessorResolver protocolAccessorResolver,
//    ITransportConnectionFactoryFactory transportResolver,
//    ILifecycleStateMachineFactory stateMachineResolver,
//    DeviceManagerFactory deviceManagerFactory,
//    PortManagerFactory portManagerFactory,
//    ReadingSinkBus readingSinkBus,
//    IEventPublisher eventPublisher,
//    ILoggerFactory loggerFactory
//    )
//{
//    public Gateway Create(GatewayDescriptor gatewayDescriptor)
//    {
//        var gatewayState = stateMachineResolver.Create(gatewayDescriptor.GatewayStateMachine);

//        var gatewayStateEventDisposable = gatewayState.StateChanged.Subscribe(x =>
//            _ = eventPublisher.PublishAsync(new GatewayStateChangedEvent(x.Old, x.New)));

//        var portManager = portManagerFactory.Create(new()
//        {
//            GatwayLifecycleState = gatewayState,
//            PortCreator = PortCreator,
//            PortBatchExecutorDescriptor = new() { Concurrency = 1 }
//        });

//        var gatewayLogger = loggerFactory.CreateLogger<Gateway>();

//        return new Gateway(
//            lifecycleStateMachine: gatewayState,
//            portManager: portManager,
//            logger: gatewayLogger
//            )
//        {
//            DisposedCallback = async () =>
//            {
//                await portManager.TryDisposeAsync();
//                gatewayStateEventDisposable.TryDispose();
//                await gatewayState.TryDisposeAsync();
//            }
//        };


//        Port PortCreator(PortDescriptor portDescriptor)
//        {
//            // 创建端口
//            var portLogger = loggerFactory.CreateLogger<Port>();
//            var portState = stateMachineResolver.Create(gatewayDescriptor.PortStateMachine);

//            var portStateEventDisposable = gatewayState.StateChanged.Subscribe(x =>
//                _ = eventPublisher.PublishAsync(new PortStateChangedEvent(portDescriptor.Id, x.Old, x.New)));

//            var transport = transportResolver.Resolve(portDescriptor.Transport);
//            var protocolAccessors = new ConcurrentDictionary<ProtocolDescriptor, IProtocolAccessor>();

//            var deviceCollection = deviceManagerFactory.Create(new()
//            {
//                PortLifecycleState = portState,
//                Port = portDescriptor.Id,
//                DevicCreator = DeviceCreator,
//                DeviceBatchExecutorDescriptor = new() { Concurrency = 3 }
//            });

//            var port = new Port(new()
//            {
//                Port = portDescriptor,
//                PortStateMachine = portState,
//                GatewayLifecycleState = gatewayState,
//                DeviceManager = deviceCollection,
//                TransportConnectionLifeCycle = transport,
//                Logger = portLogger
//            });

//            var portDisposer = DisposableCollection.From(deviceCollection, transport, portStateEventDisposable, portState);
//            port.Disposed.Subscribe(portDisposer.DisposeAsync);

//            return port;

//            Device DeviceCreator(DeviceDescriptor deviceDescriptor)
//            {
//                var deviceLogger = loggerFactory.CreateLogger<Device>();
//                var deviceState = stateMachineResolver.Create(gatewayDescriptor.DeviceStateMachine);

//                var deviceStateEventDisposable = gatewayState.StateChanged.Subscribe(x =>
//                    _ = eventPublisher.PublishAsync(new DeviceStateChangedEvent(portDescriptor.Id, deviceDescriptor.Id, x.Old, x.New)));

//                var protocolAccessor = protocolAccessors.GetOrAdd(deviceDescriptor.Protocol, x =>
//                        protocolAccessorResolver.Resolve(x, transport));

//                var subscriptionCreator = new SubscriptionCreator(new()
//                {
//                    DeviceState = deviceState,
//                    TriggerScheduler = triggerScheduler,
//                    ProtocolAccessor = protocolAccessor,
//                    PortId = portDescriptor.Id,
//                    DeviceId = deviceDescriptor.Id,
//                    ReadingSinkBus = readingSinkBus,
//                    LoggerFactory = loggerFactory,
//                    EventPublisher = eventPublisher,
//                    LifecycleStateMachineResolver = stateMachineResolver,
//                    SubscriptionDescriptor = gatewayDescriptor.SubscriptionStateMachine
//                });

//                var subscriptionCollection = new SubscriptionManager(new()
//                {
//                    DeviceState = gatewayState,
//                    SubscriptionCreator = subscriptionCreator,
//                    DeviceId = deviceDescriptor.Id,
//                    PortId = portDescriptor.Id,
//                    EventPublisher = eventPublisher
//                });

//                var device = new Device(
//                    descriptor: deviceDescriptor,
//                    lifecycleStateMachine: deviceState,
//                    portLifecycleState: portState,
//                    collection: subscriptionCollection,
//                    logger: deviceLogger);

//                var portDisposer = DisposableCollection.From(subscriptionCollection, subscriptionCreator, protocolAccessor, deviceStateEventDisposable, deviceState);
//                device.Disposed.Subscribe(async () =>
//                    await subscriptionCollection.TryDisposeAsync(deviceLogger));

//                return device;
//            }
//        }
//    }


//    internal class SubscriptionCreator(SubscriptionCreatorDependencies dependencies) :
//        ISubscriptionCreator
//    {
//        public Subscription<T> Create<T>(SubscriptionDescriptor<T> descriptor) where T : notnull
//        {
//            var subscriptionState = dependencies.LifecycleStateMachineResolver.Create(dependencies.SubscriptionDescriptor);

//            var subscriptionStateEventDisposable = subscriptionState.StateChanged.Subscribe(x =>
//                   _ = dependencies.EventPublisher.PublishAsync(new SubscriptionStateChanged(dependencies.PortId, dependencies.DeviceId, descriptor.Id, x.Old, x.New)));

//            var reader = new ReadingSource<T>(descriptor.Tag, dependencies.ProtocolAccessor, dependencies.LoggerFactory.CreateLogger<ReadingSource<T>>());

//            var subscription = new Subscription<T>(
//                descriptor: descriptor,
//                lifecycleStateMachine: subscriptionState,
//                deviceLifecycleState: dependencies.DeviceState,
//                triggerScheduler: dependencies.TriggerScheduler,
//                reader
//            );
//            var readingContext = new ReadingContext() { PortId = dependencies.PortId, DeviceId = dependencies.DeviceId, DataAddress = descriptor.Tag.Address };
//            var updateDisposable = subscription.Update.Subscribe(x => dependencies.ReadingSinkBus.Publish(readingContext, x));

//            var portDisposer = DisposableCollection.From(updateDisposable, reader, subscriptionStateEventDisposable, subscriptionState);
//            subscription.Disposed.Subscribe(portDisposer.DisposeAsync);

//            return subscription;
//        }
//    }

//    internal readonly struct SubscriptionCreatorDependencies
//    {
//        public required ILifecycleStateable DeviceState { get; init => field = value.Required(); }
//        public required ITriggerScheduler TriggerScheduler { get; init => field = value.Required(); }
//        public required IProtocolAccessor ProtocolAccessor { get; init => field = value.Required(); }
//        public required PortId PortId { get; init => field = value.Required(); }
//        public required DeviceId DeviceId { get; init => field = value.Required(); }
//        public required ReadingSinkBus ReadingSinkBus { get; init => field = value.Required(); }
//        public required ILoggerFactory LoggerFactory { get; init => field = value.Required(); }
//        public required IEventPublisher EventPublisher { get; init => field = value.Required(); }
//        public required ILifecycleStateMachineFactory LifecycleStateMachineResolver { get; init => field = value.Required(); }
//        public required LifecycleStateMachineDescriptor SubscriptionDescriptor { get; init => field = value.Required(); }
//    }
//}