using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ThabeSoft.EdgeGateway.Transports;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class SerialDependencyInjectionExtensions
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
    }
}
