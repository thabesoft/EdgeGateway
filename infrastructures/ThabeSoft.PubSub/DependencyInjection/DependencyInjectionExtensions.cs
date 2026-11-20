using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Events;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayEvents()
        {
            descriptors.TryAddSingleton<EventBus>();
            descriptors.TryAddSingleton<IEventPublisher>(sp => sp.GetRequiredService<EventBus>());
            descriptors.TryAddSingleton<IEventSubscriber>(sp => sp.GetRequiredService<EventBus>());

            return descriptors;
        }
    }
}