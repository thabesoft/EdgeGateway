using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Diagnostics.CodeAnalysis;
using ThabeSoft.Configuration;
using ThabeSoft.Triggers.Interval;
using ThabeSoft.Triggers.Signal;

namespace ThabeSoft.Triggers.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddTriggers()
        {
            descriptors.TryAddSingleton<ITriggerScheduler, TriggerScheduler>();
            descriptors.TryAddSingleton<ITriggerFactory, TriggerFactory>();


            descriptors.TryAddSingleton(TimeProvider.System);
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITriggerResolver, IntervalTriggerResolver>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITriggerResolver, SignalTriggerResolver>());

            return descriptors;
        }

        public IServiceCollection AddTriggersConfig()
        {
            descriptors.AddTriggerConfigMapper<IntervalTriggerDescriptorConfigMapper>();
            return descriptors;
        }


        private IServiceCollection AddTriggerConfigMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMapper>()
           where TMapper : class, IConfigMapper<TriggerDescriptor>
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<TriggerDescriptor>, TMapper>());
            return descriptors;
        }
    }
}