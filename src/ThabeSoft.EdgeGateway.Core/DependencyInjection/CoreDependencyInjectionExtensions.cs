using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class CoreDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGateway()
        {
            descriptors.AddEvent();
            descriptors.AddTriggers();
            descriptors.AddRecovers();
            descriptors.AddTransport();
            descriptors.AddProtocol();
            descriptors.AddReading();

            // Gateway
            descriptors.TryAddSingleton<GatewayFactory>();
            descriptors.TryAddSingleton<IGateway>(sp => sp.GetRequiredService<GatewayFactory>().Create());
            return descriptors;
        }

        public IServiceCollection AddEdgeGatewayHost()
        {
            return descriptors.AddHostedService<LaunchHostedService>();
        }

        public IServiceCollection AddEvent()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IEventPublisher, EventPublisher>());
            return descriptors;
        }

        public IServiceCollection AddTransport()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITransportConnectionFactoryResolver, TransportConnectionResolver>());
            return descriptors;
        }

        public IServiceCollection AddProtocol()
        {
            descriptors.TryAddSingleton<ProtocolAccessorResolver>();
            return descriptors;
        }

        public IServiceCollection AddRecovers()
        {
            descriptors.TryAddSingleton<PortRecoveryPolicyHandler>();
            descriptors.TryAddSingleton<RecoveryPolicyFactoryResolver>();
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IRecoveryPolicyFactory, IOExceptionRecoveryPolicyFactory>());

            return descriptors;
        }

        public IServiceCollection AddReading()
        {
            descriptors.TryAddSingleton<ReadingSinkBus>();
            descriptors.TryAddSingleton<IReadingSink, TestReadingSink>();

            return descriptors;
        }
    }
}