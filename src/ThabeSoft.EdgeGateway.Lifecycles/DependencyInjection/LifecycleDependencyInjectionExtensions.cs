using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.BackgroundTask;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines.Retryable;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines.Stric;

namespace ThabeSoft.EdgeGateway.Lifecycles.DependencyInjection;


public static class LifecycleDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayLifecycles()
        {
            // 状态机
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ILifecycleStateMachineFactory, LifecycleStateMachineFactory>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ILifecycleStateMachineResolver, StrictLifecycleStateMachineFactory>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ILifecycleStateMachineResolver, RetryableLifecycleStateMachineFactory>());


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