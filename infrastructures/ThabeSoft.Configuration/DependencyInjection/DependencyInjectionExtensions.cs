using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.Configuration.Monitor;
using ThabeSoft.Configuration.Json;
using ThabeSoft.Configuration.Nodes;
using ThabeSoft.Configuration.Paths;
using ThabeSoft.PubSub;
using Microsoft.Extensions.Logging;

namespace ThabeSoft.Configuration.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 提供Json动态解析能力
        /// </summary>
        public IServiceCollection AddJsonConfig<T>(string configFilePath, TimeSpan debouncePeriod)
        {
            descriptors.AddConfigPaths();
            descriptors.AddConfigSerializer();
            descriptors.AddJsonConfigNodeParse();
            descriptors.AddConfigFileMonitor<T>(configFilePath, debouncePeriod);

            return descriptors;
        }



        /// <summary>
        /// 提供配置文件监视能力, 通过 <see cref="IConfigMonitor{T}"/> 拿到最新数据或者订阅更新
        /// </summary>
        internal IServiceCollection AddConfigFileMonitor<T>(string configFilePath, TimeSpan debouncePeriod)
        {
            var monitorDescriptor = ServiceDescriptor.Singleton<IConfigMonitor<T>>(sp =>
            {
                var eventPublisher = sp.GetRequiredService<IEventPublisher>();
                var eventSubscriber = sp.GetRequiredService<IEventSubscriber>();
                var configSerializer = sp.GetRequiredService<IConfigSerializer>();

                var configFileWatcherLogger = sp.GetRequiredService<ILogger<ConfigFileWatcher>>();
                var configFileWatcher = new ConfigFileWatcher(configFilePath, configFileWatcherLogger) { Debounce = debouncePeriod };

                return new ConfigMonitor<T>(eventPublisher, eventSubscriber, configSerializer, configFileWatcher);
            });

            descriptors.Add(monitorDescriptor);
            return descriptors;
        }

        /// <summary>
        /// 提供配置路径解析能力
        /// </summary>
        internal IServiceCollection AddConfigPaths()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigPathParser, ConfigPathParser>());
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigPathEvaluator, ConfigPathEvaluator>());

            return descriptors;
        }
        /// <summary>
        /// 提供配置序列化能力
        /// </summary>
        internal IServiceCollection AddConfigSerializer()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigSerializer, ConfigSerializer>());
            return descriptors;
        }
        /// <summary>
        /// 提供配置解析能力
        /// </summary>
        internal IServiceCollection AddJsonConfigNodeParse()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigNodeParser, JsonConfigNodeParser>());
            return descriptors;
        }
    }
}