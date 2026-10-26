using EdgeGateway.Channel;
using EdgeGateway.Ethernet.Channel;
using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddEthernet()
        {
            descriptors.AddSingleton<IChannelFactory, TcpChannelFactory>();
            descriptors.AddSingleton<IChannelFactory, UdpChannelFactory>();

            return descriptors;
        }
    }
}
