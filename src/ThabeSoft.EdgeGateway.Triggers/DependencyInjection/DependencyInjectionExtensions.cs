using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Extensions;
using ThabeSoft.EdgeGateway.Triggers.Interval;
using ThabeSoft.EdgeGateway.Triggers.Signal;

namespace ThabeSoft.EdgeGateway.Triggers.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayTriggers()
        {
            descriptors.TryAddSingleton<ITriggerScheduler, TriggerScheduler>();
            descriptors.TryAddSingleton<ITriggerFactory, TriggerFactory>();


            descriptors.TryAddSingleton(TimeProvider.System);
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITriggerResolver, IntervalTriggerResolver>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITriggerResolver, SignalTriggerResolver>());

            return descriptors;
        }

        public IServiceCollection AddEdgeGatewayTriggersConfig()
        {
            descriptors.AddTriggerConfigMapper<IntervalTriggerDescriptorConfigMapper>();
            return descriptors;
        }
    }
}