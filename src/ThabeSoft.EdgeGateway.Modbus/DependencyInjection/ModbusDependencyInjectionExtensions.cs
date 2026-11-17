using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NModbus;
using ThabeSoft.EdgeGateway.Protocols;

namespace ThabeSoft.EdgeGateway.DependencyInjection;


public static class ModbusDependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 添加 Modbus 模块
        /// </summary>
        public IServiceCollection AddEdgeGatewayModbus()
        {
            descriptors.AddSingleton<IModbusFactory, ModbusFactory>();

            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IProtocolAccessorResolver, ModbusProtocolAccessorResolver>());
            return descriptors;
        }
    }
}