using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway;

internal sealed class GatewayFactory(
    ITriggerScheduler triggerScheduler,
    ProtocolAccessorResolver protocolAccessorResolver,
    ITransportConnectionFactoryResolver transportResolver,
    RecoveryPolicyFactoryResolver recoveryPolicyFactoryResolver,
    ReadingSinkBus readingSinkBus,
    ILoggerFactory loggerFactory
    )
{
    public Gateway Create()
    {
        var gatewayLogger = loggerFactory.CreateLogger<Gateway>();
        var gatewayState = Lifecycle.RetryableStateMachine(gatewayLogger);
        var portCollection = new PortCollection(gatwayLifecycleState: gatewayState, creator: PortCreator);
        var gateway = new Gateway(
            lifecycleStateMachine: gatewayState,
            portCollection: portCollection);
        gateway.Disposed.Subscribe(async () =>
           await portCollection.TryDisposeAsync(gatewayLogger));

        return gateway;


        Port PortCreator(PortDescriptor portDescriptor)
        {
            // 创建端口
            var portLogger = loggerFactory.CreateLogger<Port>();
            var portState = Lifecycle.RetryableStateMachine(portLogger);
            var transport = transportResolver.Resolve(portDescriptor.Transport);
            var protocolAccessors = new ConcurrentDictionary<ProtocolDescriptor, IProtocolAccessor>();
            var deviceCollection = new DeviceCollection(portState, DeviceCreator);

            var port = new Port(
                descriptor: portDescriptor,
                lifecycleStateMachine: portState,
                gatewayLifecycleState: gatewayState,
                deviceCollection: deviceCollection,
                transportConnectionLifeCycle: transport,
                logger: portLogger);
            port.Disposed.Subscribe(async () =>
            {
                await deviceCollection.TryDisposeAsync(portLogger);
                protocolAccessors.Clear();
                await transport.TryDisposeAsync(portLogger);
            });

            return port;

            Device DeviceCreator(DeviceDescriptor deviceDescriptor)
            {
                var deviceLogger = loggerFactory.CreateLogger<Device>();
                var deviceState = Lifecycle.RetryableStateMachine(deviceLogger);
                var protocolAccessor = protocolAccessors.GetOrAdd(deviceDescriptor.Protocol, x =>
                        protocolAccessorResolver.Resolve(x, transport));

                var subscriptionCreator = new SubscriptionCreator(
                    deviceState: deviceState,
                    triggerScheduler: triggerScheduler,
                    protocolAccessor: protocolAccessor,
                    portId: portDescriptor.Id,
                    deviceId: deviceDescriptor.Id,
                    readingSinkBus: readingSinkBus,
                    loggerFactory: loggerFactory
                );

                var subscriptionCollection = new SubscriptionCollection(gatewayState, subscriptionCreator);

                var device = new Device(
                    descriptor: deviceDescriptor,
                    lifecycleStateMachine: deviceState,
                    portLifecycleState: portState,
                    collection: subscriptionCollection,
                    logger: deviceLogger);
                device.Disposed.Subscribe(async () =>
                    await subscriptionCollection.TryDisposeAsync(deviceLogger));

                return device;
            }
        }
    }


    internal class SubscriptionCreator(
            ILifecycleStateable deviceState,
            ITriggerScheduler triggerScheduler,
            IProtocolAccessor protocolAccessor,
            PortId portId,
            DeviceId deviceId,
            ReadingSinkBus readingSinkBus,
            ILoggerFactory loggerFactory
            ) : ISubscriptionCreator
    {
        public Subscription<T> Create<T>(SubscriptionDescriptor<T> descriptor) where T : notnull
        {
            var subscriptionState = Lifecycle.StrictStateMachine();
            var reader = new ReadingSource<T>(descriptor.Tag, protocolAccessor, loggerFactory.CreateLogger<ReadingSource<T>>());

            var subscription = new Subscription<T>(
                descriptor: descriptor,
                lifecycleStateMachine: subscriptionState,
                deviceLifecycleState: deviceState,
                triggerScheduler: triggerScheduler,
                reader
            );
            var readingContext = new ReadingContext() { PortId = portId, DeviceId = deviceId, DataAddress = descriptor.Tag.Address };
            var updateDisposable = subscription.Update.Subscribe(x => readingSinkBus.Publish(readingContext, x));
            subscription.Disposed.Subscribe(async () =>
            {
                updateDisposable.Dispose();
                await subscriptionState.TryDisposeAsync();
            });

            return subscription;
        }
    }

}