using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.ComponentModel.Design;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Readings;
using ThabeSoft.EdgeGateway.Recoveries;
using ThabeSoft.EdgeGateway.Transport;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
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