using Microsoft.Extensions.DependencyInjection;
using NModbus;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway;


public static class ModbusDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加 Modbus 模块
        /// </summary>
        public IServiceCollection AddModbus()
        {
            descriptors.AddSingleton<IModbusFactory, ModbusFactory>();
            descriptors.AddSingleton<IProtocolAccessorFactory, ModbusProtocolAccessorFactory>();
            return descriptors;
        }
    }
}