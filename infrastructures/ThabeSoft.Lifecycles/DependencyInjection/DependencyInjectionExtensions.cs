using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.Lifecycles.BackgroundTask;
using ThabeSoft.Lifecycles.Collections;
using ThabeSoft.Lifecycles.StateMachines;
using ThabeSoft.Lifecycles.StateMachines.Retryable;
using ThabeSoft.Lifecycles.StateMachines.Stric;

namespace ThabeSoft.Lifecycles.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 提供生命周期创建能力
        /// </summary>
        public IServiceCollection AddLifecycles()
        {
            // 状态机
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ILifecycleStateMachineFactory, LifecycleStateMachineFactory>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ILifecycleStateMachineResolver, StrictLifecycleStateMachineFactory>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ILifecycleStateMachineResolver, RetryableLifecycleStateMachineResolver>());


            // 集合
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ILifecycleKeyedCollectionFactory, LifecycleKeyedCollectionFactory>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ILifecycleBatchExecutorFactory, LifecycleBatchExecutorFactory>());


            // 生命周期
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ILifecycleFactory, LifecycleFactory>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ILifecycleResolver, BakgroundTaskLifecycleResolver>());


            return descriptors;
        }
    }
}