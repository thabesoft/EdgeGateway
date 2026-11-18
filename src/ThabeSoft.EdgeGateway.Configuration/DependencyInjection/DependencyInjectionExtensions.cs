using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Configuration.Json;
using ThabeSoft.EdgeGateway.Configuration.Mappers;
using ThabeSoft.EdgeGateway.Configuration.Models;
using ThabeSoft.EdgeGateway.Devices;
using ThabeSoft.EdgeGateway.Ports;
using ThabeSoft.EdgeGateway.Subscription;
using ThabeSoft.EdgeGateway.Subscriptions;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Configuration.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEdgeGatewayJsonConfiguration()
        {
            //descriptors.TryAddSingleton(_ => new ConfigFileWatcher("Gateway.json", TimeSpan.FromSeconds(1)));
            //descriptors.TryAddSingleton<ConfigManager>();
            //descriptors.AddHostedService<JsonFileConfigHostedService>();
            //descriptors.TryAddSingleton<IConfigSerializer, JsonConfigSerializer>();

            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigSerializer, JsonConfigSerializer>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigNodeParser, JsonConfigNodeParser>());

            return descriptors;
        }


        public IServiceCollection AddAddEdgeGatewayConifigMappers()
        {
            descriptors.AddSingleton<IConfigMapper<GatewayConfig>, GatewayConfigMapper>();

            // Port
            descriptors.AddSingleton<IConfigMapper<PortConfig>, PortConfigMapper>();
            descriptors.AddSingleton<IConfigMapper<PortDescriptor>, PortDescriptorConfigMapper>();

            // Retry
            descriptors.AddSingleton<IConfigMapper<RetryDescriptor>, RetyrDescriptorConfigMapper>();


            // Device
            descriptors.AddSingleton<IConfigMapper<DeviceConfig>, DeviceConfigMapper>();
            descriptors.AddSingleton<IConfigMapper<DeviceDescriptor>, DeviceDescriptorConfigMapper>();

            // Subscription
            descriptors.AddSingleton<IConfigMapper<SubscriptionConfig>, SubscriptionConfigMapper>();

            descriptors.AddSingleton<IConfigMapper<SubscriptionDescriptor>, SubscriptionDescriptorConfigMapper>();
            descriptors.AddSingleton<IConfigMapper<SubscriptionDescriptor<bool>>, SubscriptionDescriptorConfigMapper<bool>>();
            descriptors.AddSingleton<IConfigMapper<SubscriptionDescriptor<int>>, SubscriptionDescriptorConfigMapper<int>>();



            return descriptors;
        }
    }
}