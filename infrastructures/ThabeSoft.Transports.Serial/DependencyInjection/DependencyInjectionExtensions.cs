using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.Transports.DependencyInjection;

namespace ThabeSoft.Transports.Serial.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 提供串行传输能力
        /// </summary>
        public IServiceCollection AddSerialTransport()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITransportConnectionResolver, SerialTransportResolver>());
            return descriptors;
        }

        /// <summary>
        /// 提供串行传输配置映射能力
        /// </summary>
        public IServiceCollection AddSerialTransportConfig()
        {
            descriptors.AddTransportConfigMapper<SerialTransportDescriptorConfigMapper>();
            return descriptors;
        }
    }
}
