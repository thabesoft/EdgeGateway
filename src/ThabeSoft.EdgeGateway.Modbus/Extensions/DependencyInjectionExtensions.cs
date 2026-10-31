using EdgeGateway.Tag;
using Microsoft.Extensions.DependencyInjection;
using NModbus;
using ThabeSoft.EdgeGateway.Modbus;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace EdgeGateway;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加 Modbus 模块
        /// </summary>
        public IServiceCollection AddModbus()
        {
            descriptors.AddSingleton<IModbusFactory, ModbusFactory>();
            descriptors.AddSingleton<ITagReaderFactory, ModbusTagReaderFactory>();
            descriptors.AddSingleton<ITagWriterFactory, ModbusTagWriterFactory>();
            return descriptors;
        }
    }
}
