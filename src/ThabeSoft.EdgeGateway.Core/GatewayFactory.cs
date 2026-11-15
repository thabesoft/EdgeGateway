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
    TransportConnectionResolver transportResolver,
    RecoveryPolicyFactoryResolver recoveryPolicyFactoryResolver,
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


        Port PortCreator(PortDescriptor descriptor)
        {
            // 创建端口
            var portLogger = loggerFactory.CreateLogger<Port>();
            var portState = Lifecycle.RetryableStateMachine(portLogger);
            var transport = transportResolver.Resolve(descriptor.Transport);
            var protocolAccessors = new ConcurrentDictionary<ProtocolDescriptor, IProtocolAccessor>();
            var recoverHandler = new PortRecoveryPolicyHandler(descriptor.Recoveries.Select(recoveryPolicyFactoryResolver.Resolve));
            var deviceCollection = new DeviceCollection(portState, DeviceCreator);

            var port = new Port(
                descriptor: descriptor,
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

            Device DeviceCreator(DeviceDescriptor descriptor)
            {
                var deviceLogger = loggerFactory.CreateLogger<Device>();
                var deviceState = Lifecycle.RetryableStateMachine(deviceLogger);
                var protocolAccessor = protocolAccessors.GetOrAdd(descriptor.Protocol, x =>
                        protocolAccessorResolver.Resolve(x, transport));

                var subscriptionCreator = new SubscriptionCreator(
                    deviceState: deviceState,
                    triggerScheduler: triggerScheduler,
                    protocolAccessor: protocolAccessor,
                    loggerFactory: loggerFactory
                );

                var subscriptionCollection = new SubscriptionCollection(gatewayState, subscriptionCreator);

                var device = new Device(
                    descriptor: descriptor,
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
            subscription.Disposed.Subscribe(async () => await subscriptionState.TryDisposeAsync());

            return subscription;
        }
    }

}