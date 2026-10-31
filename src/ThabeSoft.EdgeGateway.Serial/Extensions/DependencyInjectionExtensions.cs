using ThabeSoft.EdgeGateway.Channel;
using ThabeSoft.EdgeGateway.Serial.Channel;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加串口模块
        /// </summary>
        public IServiceCollection AddSerial()
        {
            descriptors.AddSingleton<IChannelFactory, SerialChannelFactory>();
            return descriptors;
        }
    }
}
