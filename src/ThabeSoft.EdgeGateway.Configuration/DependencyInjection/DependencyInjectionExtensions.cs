using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Configuration.Json;
using ThabeSoft.EdgeGateway.Configuration.Mappers;
using ThabeSoft.EdgeGateway.Configuration.Models;
using ThabeSoft.EdgeGateway.Configuration.Nodes;
using ThabeSoft.EdgeGateway.Configuration.Paths;
using ThabeSoft.EdgeGateway.Configuration.Rloader;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Configuration.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 提供配置路径解析能力
        /// </summary>
        public IServiceCollection AddEdgeGatewayConfigPaths()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigPathParser, ConfigPathParser>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigPathEvaluator, ConfigPathEvaluator>());

            return descriptors;
        }

        /// <summary>
        /// 提供配置文件自动加载能力
        /// </summary>
        public IServiceCollection AddEdgeGatewayConfigurationReloader()
        {
            descriptors.AddOptions<ConfigReloadOptions>()
                .BindConfiguration(ConfigReloadOptions.SectionName)
                .Validate(opts =>
                {
                    if (string.IsNullOrWhiteSpace(opts.FilePath))
                        return false;

                    if (opts.DebouncePeriod.TotalMilliseconds < 10)
                        return false;

                    return true;
                })
                .ValidateOnStart();


            descriptors.TryAddSingleton<ConfigFileWatcher>();
            descriptors.TryAddSingleton<ConfigApplier>();
            descriptors.AddHostedService<ConfigFileReloadHostedService>();
            return descriptors;
        }

        /// <summary>
        /// 提供配置解析能力
        /// </summary>
        public IServiceCollection AddEdgeGatewayJsonConfigProvider()
        {
            descriptors.TryAddSingleton<IConfigSerializer, JsonConfigSerializer>();
            descriptors.TryAddSingleton<IConfigNodeParser, JsonConfigNodeParser>();

            return descriptors;
        }

        /// <summary>
        /// 提供配置解析映射能力
        /// </summary>
        public IServiceCollection AddEdgeGatewayConfigMappers()
        {
            // Gateway
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<GatewayConfig>, GatewayConfigMapper>());
            // Retry
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigMapper<RetryDescriptor>, RetryDescriptorConfigMapper>());

            return descriptors;
        }
    }
}