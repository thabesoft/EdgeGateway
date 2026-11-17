using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transports;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.Utilities;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway;


internal sealed class GatewayFactory(
    ITriggerScheduler triggerScheduler,
    ProtocolAccessorResolver protocolAccessorResolver,
    ITransportConnectionFactoryResolver transportResolver,
    ReadingSinkBus readingSinkBus,
    IEventPublisher eventPublisher,
    ILoggerFactory loggerFactory
    )
{
    public Gateway Create()
    {
        var gatewayLogger = loggerFactory.CreateLogger<Gateway>();
        var gatewayState = Lifecycle.RetryableStateMachine(gatewayLogger);

        var gatewayStateEventDisposable = gatewayState.StateChanged.Subscribe(x =>
            _ = eventPublisher.PublishAsync(new GatewayStateChangedEvent(x.Old, x.New)));

        var portManager = new PortManager(
            gatwayLifecycleState: gatewayState,
            eventPublisher: eventPublisher,
            creator: PortCreator);

        var gateway = new Gateway(
            lifecycleStateMachine: gatewayState,
            portManager: portManager);

        var gatewayDisposer = DisposableCollection.From(portManager, gatewayStateEventDisposable, gatewayState);
        gateway.Disposed.Subscribe(gatewayDisposer.DisposeAsync);

        return gateway;


        Port PortCreator(PortDescriptor portDescriptor)
        {
            // 创建端口
            var portLogger = loggerFactory.CreateLogger<Port>();
            var portState = Lifecycle.RetryableStateMachine(portLogger);

            var portStateEventDisposable = gatewayState.StateChanged.Subscribe(x =>
                _ = eventPublisher.PublishAsync(new PortStateChangedEvent(portDescriptor.Id, x.Old, x.New)));

            var transport = transportResolver.Resolve(portDescriptor.Transport);
            var protocolAccessors = new ConcurrentDictionary<ProtocolDescriptor, IProtocolAccessor>();

            var deviceCollection = new DeviceCollection(new()
            {
                PortLifecycleState = portState,
                PortId = portDescriptor.Id,
                EventPublisher = eventPublisher,
                DevicCreator = DeviceCreator
            });

            var port = new Port(new()
            {
                Descriptor = portDescriptor,
                LifecycleStateMachine = portState,
                GatewayLifecycleState = gatewayState,
                DeviceCollection = deviceCollection,
                TransportConnectionLifeCycle = transport,
                Logger = portLogger
            });

            var portDisposer = DisposableCollection.From(deviceCollection, transport, portStateEventDisposable, portState);
            port.Disposed.Subscribe(portDisposer.DisposeAsync);

            return port;

            Device DeviceCreator(DeviceDescriptor deviceDescriptor)
            {
                var deviceLogger = loggerFactory.CreateLogger<Device>();
                var deviceState = Lifecycle.RetryableStateMachine(deviceLogger);

                var deviceStateEventDisposable = gatewayState.StateChanged.Subscribe(x =>
                    _ = eventPublisher.PublishAsync(new DeviceStateChangedEvent(portDescriptor.Id, deviceDescriptor.Id, x.Old, x.New)));

                var protocolAccessor = protocolAccessors.GetOrAdd(deviceDescriptor.Protocol, x =>
                        protocolAccessorResolver.Resolve(x, transport));

                var subscriptionCreator = new SubscriptionCreator(new()
                {
                    DeviceState = deviceState,
                    TriggerScheduler = triggerScheduler,
                    ProtocolAccessor = protocolAccessor,
                    PortId = portDescriptor.Id,
                    DeviceId = deviceDescriptor.Id,
                    ReadingSinkBus = readingSinkBus,
                    LoggerFactory = loggerFactory,
                    EventPublisher = eventPublisher
                });

                var subscriptionCollection = new SubscriptionCollection(new()
                {
                    GatwayLifecycleState = gatewayState,
                    SubscriptionCreator = subscriptionCreator,
                    DeviceId = deviceDescriptor.Id,
                    PortId = portDescriptor.Id,
                    EventPublisher = eventPublisher
                });

                var device = new Device(
                    descriptor: deviceDescriptor,
                    lifecycleStateMachine: deviceState,
                    portLifecycleState: portState,
                    collection: subscriptionCollection,
                    logger: deviceLogger);

                var portDisposer = DisposableCollection.From(subscriptionCollection, subscriptionCreator, protocolAccessor, deviceStateEventDisposable, deviceState);
                device.Disposed.Subscribe(async () =>
                    await subscriptionCollection.TryDisposeAsync(deviceLogger));

                return device;
            }
        }
    }


    internal class SubscriptionCreator(SubscriptionCreatorDependencies dependencies) :
        ISubscriptionCreator
    {
        public Subscription<T> Create<T>(SubscriptionDescriptor<T> descriptor) where T : notnull
        {
            var subscriptionState = Lifecycle.StrictStateMachine();

            var subscriptionStateEventDisposable = subscriptionState.StateChanged.Subscribe(x =>
                   _ = dependencies.EventPublisher.PublishAsync(new SubscriptionStateChanged(dependencies.PortId, dependencies.DeviceId, descriptor.Id, x.Old, x.New)));

            var reader = new ReadingSource<T>(descriptor.Tag, dependencies.ProtocolAccessor, dependencies.LoggerFactory.CreateLogger<ReadingSource<T>>());

            var subscription = new Subscription<T>(
                descriptor: descriptor,
                lifecycleStateMachine: subscriptionState,
                deviceLifecycleState: dependencies.DeviceState,
                triggerScheduler: dependencies.TriggerScheduler,
                reader
            );
            var readingContext = new ReadingContext() { PortId = dependencies.PortId, DeviceId = dependencies.DeviceId, DataAddress = descriptor.Tag.Address };
            var updateDisposable = subscription.Update.Subscribe(x => dependencies.ReadingSinkBus.Publish(readingContext, x));

            var portDisposer = DisposableCollection.From(updateDisposable, reader, subscriptionStateEventDisposable, subscriptionState);
            subscription.Disposed.Subscribe(portDisposer.DisposeAsync);

            return subscription;
        }
    }

    internal readonly struct SubscriptionCreatorDependencies
    {
        public required ILifecycleStateable DeviceState { get; init => field = value.Required(); }
        public required ITriggerScheduler TriggerScheduler { get; init => field = value.Required(); }
        public required IProtocolAccessor ProtocolAccessor { get; init => field = value.Required(); }
        public required PortId PortId { get; init => field = value.Required(); }
        public required DeviceId DeviceId { get; init => field = value.Required(); }
        public required ReadingSinkBus ReadingSinkBus { get; init => field = value.Required(); }
        public required ILoggerFactory LoggerFactory { get; init => field = value.Required(); }
        public required IEventPublisher EventPublisher { get; init => field = value.Required(); }
    }
}