using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Lifecycles.Collections;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines.Retryable;
using ThabeSoft.EdgeGateway.Lifecycles.StateMachines.Stric;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class LifecycleDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayTriggers()
        {
            descriptors.TryAddSingleton<ILifecycleStateMachineFactory, LifecycleStateMachineResolver>();


            // 集合
            descriptors.TryAddSingleton<ILifecycleKeyedCollectionFactory, LifecycleKeyedCollectionFactory>();
            descriptors.TryAddSingleton<ILifecycleBatchExecutorFactory, LifecycleBatchExecutorFactory>();


            // 默认实现状态机
            descriptors.TryAddSingleton<ILifecycleStateMachineResolver, StrictLifecycleStateMachineFactory>();
            descriptors.TryAddSingleton<ILifecycleStateMachineResolver, RetryableLifecycleStateMachineFactory>();


            return descriptors;
        }
    }
}