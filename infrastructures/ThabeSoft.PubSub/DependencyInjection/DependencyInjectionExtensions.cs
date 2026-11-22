using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
            descriptors.TryAddSingleton<EventBus>();
            descriptors.TryAddSingleton<IEventPublisher>(sp => sp.GetRequiredService<EventBus>());
            descriptors.TryAddSingleton<IEventSubscriber>(sp => sp.GetRequiredService<EventBus>());

            return descriptors;
        }
    }
}