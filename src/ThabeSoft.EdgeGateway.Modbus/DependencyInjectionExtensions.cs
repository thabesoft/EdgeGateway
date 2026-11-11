using Microsoft.Extensions.DependencyInjection;
using NModbus;
using ThabeSoft.EdgeGateway.Modbus.Internal;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.Modbus;


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
            descriptors.AddSingleton<IProtocolAccessorFactory, ModbusProtocolAccessorFactory>();
            return descriptors;
        }
    }
}