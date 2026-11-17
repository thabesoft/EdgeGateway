using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.Triggers.Interval;
using ThabeSoft.EdgeGateway.Triggers.Signal;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class TriggerDependencyInjectionExtensions
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
    }
}