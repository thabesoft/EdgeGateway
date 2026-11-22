using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.Configuration.Monitor;
using ThabeSoft.Configuration.Json;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.Configuration.Paths;

namespace ThabeSoft.Configuration.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 提供配置路径解析能力
        /// </summary>
        public IServiceCollection AddConfigPaths()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigPathParser, ConfigPathParser>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigPathEvaluator, ConfigPathEvaluator>());

            return descriptors;
        }

        /// <summary>
        /// 提供配置文件自动加载能力
        /// </summary>
        public IServiceCollection AddConfigMonitor<T>(string optionSectionName = "Config")
        {
            descriptors.AddOptions<ConfigFileWatcherOptions>()
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
            descriptors.TryAddSingleton<ConfigMonitor<T>>();
            return descriptors;
        }

        /// <summary>
        /// 提供配置序列化能力
        /// </summary>
        public IServiceCollection AddConfigSerializer()
        {
            descriptors.TryAddSingleton<IConfigSerializer, ConfigSerializer>();
            return descriptors;
        }

        /// <summary>
        /// 提供配置解析能力
        /// </summary>
        public IServiceCollection AddJsonConfigNodeParse()
        {
            descriptors.TryAddSingleton<IConfigNodeParser, JsonConfigNodeParser>();
            return descriptors;
        }
    }
}