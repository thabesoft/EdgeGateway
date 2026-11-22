using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.Triggers.Interval;
using ThabeSoft.Triggers.Signal;

namespace ThabeSoft.Triggers.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加触发器
        /// </summary>
        public IServiceCollection AddTriggers()
        {
            descriptors.TryAddSingleton<ITriggerScheduler, TriggerScheduler>();
            descriptors.TryAddSingleton<ITriggerFactory, TriggerFactory>();


            descriptors.TryAddSingleton(TimeProvider.System);
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITriggerResolver, IntervalTriggerResolver>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITriggerResolver, SignalTriggerResolver>());

            return descriptors;
        }

        /// <summary>
        /// 添加触发器配置映射
        /// </summary>
        public IServiceCollection AddTriggersConfig()
        {
            descriptors.AddTriggerConfigMapper<IntervalTriggerDescriptorConfigMapper>();
            return descriptors;
        }
    }
}