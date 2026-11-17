using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class CoreDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayHost()
        {
            return descriptors.AddHostedService<LaunchHostedService>();
        }

        public IServiceCollection AddEdgeGateway(GatewayCreationArgs creationContext)
        {
            descriptors.TryAddSingleton<GatewayFactory>();
            descriptors.TryAddSingleton<IGateway>(sp =>
            {
                var gatewayFactory = sp.GetRequiredService<GatewayFactory>();
                return gatewayFactory.Create(creationContext);
            });
            return descriptors;
        }
        public IServiceCollection AddEdgeGatewayPorts()
        {
            descriptors.TryAddSingleton<PortFactory>();
            descriptors.TryAddSingleton<PortManagerFactory>();

            return descriptors;
        }
        public IServiceCollection AddEdgeGatewayDevices()
        {
            descriptors.TryAddSingleton<DeviceFactory>();
            descriptors.TryAddSingleton<DeviceManagerFactory>();

            return descriptors;
        }
        public IServiceCollection AddEdgeGatewaySubscriptions()
        {
            descriptors.TryAddSingleton<SubscriptionFactory>();
            descriptors.TryAddSingleton<SubscriptionManagerFactory>();

            return descriptors;
        }


        public IServiceCollection AddEdgeGatewayEvents()
        {
            descriptors.TryAddSingleton<EventBus>();
            descriptors.TryAddSingleton<IEventPublisher>(sp => sp.GetRequiredService<EventBus>());
            descriptors.TryAddSingleton<IEventSubscriber>(sp => sp.GetRequiredService<EventBus>());

            return descriptors;
        }
        public IServiceCollection AddEdgeGatewayTransport()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITransportConnectionFactory, TransportConnectionFactory>());
            return descriptors;
        }

        public IServiceCollection AddEdgeGatewayProtocol()
        {
            descriptors.TryAddSingleton<ProtocolAccessorFactory>();
            return descriptors;
        }

        public IServiceCollection AddEdgeGatewayReadings()
        {
            descriptors.TryAddSingleton<ReadingSinkBus>();
            descriptors.TryAddSingleton<IReadingSink, TestReadingSink>();

            return descriptors;
        }
    }
}