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
        public IServiceCollection AddTriggers()
        {
            descriptors.TryAddSingleton<ITriggerScheduler, TriggerScheduler>();
            descriptors.TryAddSingleton<ITriggerResolver, TriggerResolver>();


            descriptors.TryAddSingleton(TimeProvider.System);
            descriptors.TryAddSingleton<ITriggerFactory, FixedIntervalTriggerFactory>();
            descriptors.TryAddSingleton<ITriggerFactory, SignalTriggerFactory>();

            return descriptors;
        }
    }
}