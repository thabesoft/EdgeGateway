using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Triggers;
using ThabeSoft.EdgeGateway.Triggers.Interval;
using ThabeSoft.EdgeGateway.Triggers.Signal;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddTriggers()
        {
            descriptors.TryAddSingleton<ITriggerScheduler, TriggerScheduler>();
            descriptors.TryAddSingleton<ITriggerFactorySelector, TriggerFactorySelector>();


            descriptors.TryAddSingleton(TimeProvider.System);
            descriptors.TryAddSingleton<ITriggerFactory, FixedIntervalTriggerFactory>();
            descriptors.TryAddSingleton<ITriggerFactory, SignalTriggerFactory>();

            return descriptors;
        }
    }
}