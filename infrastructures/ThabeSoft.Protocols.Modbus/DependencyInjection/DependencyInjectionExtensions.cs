using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NModbus;
using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.Protocols;

namespace ThabeSoft.EdgeGateway.Modbus.DependencyInjection;


public static class DependencyInjectionExtensions
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


        public IServiceCollection AddEdgeGatewayModbusConfig()
        {
            descriptors.AddProtocolConfigMapper<ModbusProtocolDescriptorConfigMapper>();
            descriptors.AddDataAddressMapper<ModbusDataAddressConfigMapper>();
            return descriptors;
        }
    }
}