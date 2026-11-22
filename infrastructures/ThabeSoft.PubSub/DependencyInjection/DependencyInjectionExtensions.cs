using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.PubSub.InMemory;

namespace ThabeSoft.PubSub.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 提供订阅发布消息能力
        /// </summary>
        public IServiceCollection AddPubSub()
        {
            descriptors.TryAddSingleton<InMemoryEventBus>();
            descriptors.TryAddSingleton<IEventPublisher>(sp => sp.GetRequiredService<InMemoryEventBus>());
            descriptors.TryAddSingleton<IEventSubscriber>(sp => sp.GetRequiredService<InMemoryEventBus>());

            return descriptors;
        }
    }
}