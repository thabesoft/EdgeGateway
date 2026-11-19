using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.Serial.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加串口模块
        /// </summary>
        public IServiceCollection AddEdgeGatewaySerial()
        {
            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<ITransportConnectionResolver, SerialTransportResolver>());
            return descriptors;
        }


        public IServiceCollection AddEdgeGatewaySerialConfig()
        {
            descriptors.AddTransportConfigMapper<SerialTransportDescriptorConfigMapper>();
            return descriptors;
        }
    }
}
