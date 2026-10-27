using EdgeGateway.Config;
using EdgeGateway.Serial;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class ConfigBuilderExtensions
{
    extension(PortConfigBuilder portConfigBuilder)
    {
        /// <summary>
        /// 使用串口通道
        /// </summary>
        /// <param name="serialPortName">串口名称</param>
        /// <exception cref="ArgumentException"></exception>
        public PortConfigBuilder Serial(string serialPortName)
        {
            var channelDescriptor = SerialChannelDescriptor.Create(serialPortName);
            return portConfigBuilder.SetDescriptor(channelDescriptor);
        }
    }
}