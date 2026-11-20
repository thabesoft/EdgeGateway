using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.Configuration.Json;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.Configuration.Paths;
using ThabeSoft.Configuration.Rloader;

namespace ThabeSoft.Configuration.DependencyInjection;


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
        public IServiceCollection AddEdgeGatewayConfigurationReloader(string optionSectionName)
        {
            descriptors.AddOptions<ConfigReloadOptions>()
                .BindConfiguration(optionSectionName)
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
            descriptors.AddHostedService<ConfigFileReloadHostedService>();
            return descriptors;
        }

        /// <summary>
        /// 提供配置解析能力
        /// </summary>
        public IServiceCollection AddEdgeGatewayJsonConfigProvider()
        {
            descriptors.TryAddSingleton<IConfigNodeParser, JsonConfigNodeParser>();

            return descriptors;
        }
    }
}