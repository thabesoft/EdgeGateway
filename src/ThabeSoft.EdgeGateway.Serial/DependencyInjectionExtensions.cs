using ThabeSoft.EdgeGateway.Transport;
using Microsoft.Extensions.DependencyInjection;
using ThabeSoft.EdgeGateway.Serial.Internal;

namespace ThabeSoft.EdgeGateway.Serial;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加串口模块
        /// </summary>
        public IServiceCollection AddSerial()
        {
            descriptors.AddSingleton<ITransportConnectionFactory, SerialTransportFactory>();
            return descriptors;
        }
    }
}
