using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.PubSub;
using ThabeSoft.PubSub.InMemory;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 提供内存版本的订阅发消息能力
        /// </summary>
        public IServiceCollection AddInMemoryPubSub()
        {
            descriptors.TryAddSingleton<InMemoryEventBus>();
            descriptors.TryAddSingleton<IEventPublisher>(sp => sp.GetRequiredService<InMemoryEventBus>());
            descriptors.TryAddSingleton<IEventSubscriber>(sp => sp.GetRequiredService<InMemoryEventBus>());

            return descriptors;
        }
    }
}