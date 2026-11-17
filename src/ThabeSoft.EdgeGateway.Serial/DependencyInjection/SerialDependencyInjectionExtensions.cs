using ThabeSoft.EdgeGateway.Transports;
using Microsoft.Extensions.DependencyInjection;

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
            descriptors.AddSingleton<ITransportConnectionResolver, SerialTransportFactory>();
            return descriptors;
        }
    }
}
