using Microsoft.Extensions.DependencyInjection;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加 三菱 模块
        /// </summary>
        public IServiceCollection AddMitsubishi()
        {
            //descriptors.AddSingleton<IDeviceFactory, ModbusDeviceFactory>();
            return descriptors;
        }
    }
}
