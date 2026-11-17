using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class CoreDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGateway(GatewayCreationContext creationContext)
        {
            descriptors.AddSingleton<SubscriptionFactory>();
            descriptors.AddSingleton<SubscriptionManagerFactory>();

            descriptors.AddSingleton<DeviceFactory>();
            descriptors.AddSingleton<DeviceManagerFactory>();

            descriptors.AddSingleton<PortFactory>();
            descriptors.AddSingleton<PortManagerFactory>();

            descriptors.AddSingleton<GatewayFactory>();


            descriptors.AddEdgeGatewayEvents();
            descriptors.AddEdgeGatewayRecovers();
            descriptors.AddEdgeGatewayTransport();
            descriptors.AddEdgeGatewayProtocol();
            descriptors.AddEdgeGatewayReadings();

            // Gateway
            descriptors.TryAddSingleton<GatewayFactory>();
            descriptors.TryAddSingleton<IGateway>(sp =>
            {
                var gatewayFactory = sp.GetRequiredService<GatewayFactory>();
                return gatewayFactory.Create(creationContext);
            });

            return descriptors;
        }

        public IServiceCollection AddEdgeGatewayHost()
        {
            return descriptors.AddHostedService<LaunchHostedService>();
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

        public IServiceCollection AddEdgeGatewayRecovers()
        {
            descriptors.TryAddSingleton<PortRecoveryPolicyHandler>();
            descriptors.TryAddSingleton<RecoveryPolicyFactoryResolver>();
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IRecoveryPolicyFactory, IOExceptionRecoveryPolicyFactory>());

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