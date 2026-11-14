using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Events;
using ThabeSoft.EdgeGateway.Hosting;
using ThabeSoft.EdgeGateway.Protocols;
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
            // Events
            descriptors.TryAddSingleton<IEventPublisher, EventPublisher>();

            // Modules
            descriptors.AddTriggers();
            descriptors.AddRecovers();
            descriptors.AddTransport();

            // Resolvers
            descriptors.TryAddSingleton<TransportConnectionResolver>();
            descriptors.TryAddSingleton<ProtocolAccessorResolver>();
            descriptors.TryAddSingleton<RecoveryPolicyFactoryResolver>();


            // Gateway
            descriptors.TryAddSingleton<GatewayFactory>();
            descriptors.TryAddSingleton<IGateway>(sp => sp.GetRequiredService<GatewayFactory>().Create());
            return descriptors;
        }

        public IServiceCollection AddEdgeGatewayHost()
        {
            return descriptors.AddHostedService<LaunchHostedService>();
        }


        public IServiceCollection AddTransport()
        {
            descriptors.AddSingleton<ITransportConnectionFactoryResolver, TransportConnectionResolver>();
            return descriptors;
        }

        public IServiceCollection AddRecovers()
        {
            descriptors.TryAddSingleton<PortRecoveryPolicyHandler>();
            descriptors.TryAddSingleton<RecoveryPolicyFactoryResolver>();
            descriptors.TryAddSingleton<IRecoveryPolicyFactory, IOExceptionRecoveryPolicyFactory>();

            return descriptors;
        }
    }
}