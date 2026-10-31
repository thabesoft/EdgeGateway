using ThabeSoft.EdgeGateway.Channel;
using ThabeSoft.EdgeGateway.Ethernet.Channel;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace ThabeSoft.EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加以太网模块
        /// </summary>
        public IServiceCollection AddEthernet()
        {
            descriptors.AddSingleton<IChannelFactory, TcpChannelFactory>();
            descriptors.AddSingleton<IChannelFactory, UdpChannelFactory>();

            return descriptors;
        }
    }
}
