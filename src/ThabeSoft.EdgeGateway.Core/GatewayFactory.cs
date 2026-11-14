using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transport;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway;

internal sealed class GatewayFactory(
    ITriggerScheduler triggerScheduler,
    ProtocolAccessorResolver protocolAccessorResolver,
    TransportConnectionResolver transportResolver,
    RecoveryPolicyFactoryResolver recoveryPolicyFactoryResolver
    )
{
    public Gateway Create()
    {
        var gatewayState = Lifecycle.RetryableStateMachine();
        var portCollection = new PortCollection(gatwayLifecycleState: gatewayState, creator: PortCreator);
        var gateway = new Gateway(
            lifecycleStateMachine: gatewayState,
            portCollection: portCollection);
        gateway.Disposed.Subscribe(async () =>
           await portCollection.TryDisposeAsync());

        return gateway;


        Port PortCreator(PortDescriptor descriptor)
        {
            // 创建端口
            var portState = Lifecycle.RetryableStateMachine();
            var transport = transportResolver.Resolve(descriptor.Transport);
            var protocolAccessors = new ConcurrentDictionary<IProtocolDescriptor, IProtocolAccessor>();
            var recoverHandler = new PortRecoveryPolicyHandler(descriptor.Recoveries.Select(recoveryPolicyFactoryResolver.Resolve));
            var deviceCollection = new DeviceCollection(portState, DeviceCreator);

            var port = new Port(
                descriptor: descriptor,
                lifecycleStateMachine: portState,
                gatewayLifecycleState: gatewayState,
                deviceCollection: deviceCollection,
                transportConnectionLifeCycle: transport);
            port.Disposed.Subscribe(async () =>
            {
                await deviceCollection.TryDisposeAsync();
                protocolAccessors.Clear();
                await transport.TryDisposeAsync();
            });

            return port;

            Device DeviceCreator(DeviceDescriptor descriptor)
            {
                var deviceState = Lifecycle.RetryableStateMachine();
                var protocolAccessor = protocolAccessors.GetOrAdd(descriptor.Protocol, x =>
                        protocolAccessorResolver.Resolve(x, transport));

                var subscriptionCreator = new SubscriptionCreator(
                    deviceState: deviceState,
                    triggerScheduler: triggerScheduler,
                    protocolAccessor: protocolAccessor,
                    recover: recoverHandler.Handle
                );

                var subscriptionCollection = new SubscriptionCollection(gatewayState, subscriptionCreator);

                var device = new Device(
                    descriptor: descriptor,
                    lifecycleStateMachine: deviceState,
                    portLifecycleState: portState,
                    collection: subscriptionCollection);
                device.Disposed.Subscribe(async () =>
                    await subscriptionCollection.TryDisposeAsync());

                return device;
            }
        }
    }


    internal class SubscriptionCreator(
            ILifecycleStateable deviceState,
            ITriggerScheduler triggerScheduler,
            IProtocolAccessor protocolAccessor,
            Func<Exception, bool> recover
            ) : ISubscriptionCreator
    {
        public Subscription<T> Create<T>(SubscriptionDescriptor<T> descriptor) where T : notnull
        {
            var subscriptionState = Lifecycle.StrictStateMachine();
            var reader = new SubscriptionReader<T>(descriptor.Tag, protocolAccessor, recover);

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