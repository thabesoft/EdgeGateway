using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NModbus;
using ThabeSoft.Protocols.Accessor;
using ThabeSoft.Protocols.DependencyInjection;

namespace ThabeSoft.Protocols.Modbus.DependencyInjection;


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        /// <summary>
        /// 提供 Modbus 协议访问能力
        /// </summary>
        public IServiceCollection AddModbusProtocol()
        {
            descriptors.AddSingleton<IModbusFactory, ModbusFactory>();

            descriptors.TryAddEnumerable(ServiceDescriptor.Singleton<IProtocolAccessorResolver, ModbusProtocolAccessorResolver>());
            return descriptors;
        }

        /// <summary>
        /// 提供 Modbus 协议配置解析能力
        /// </summary>
        public IServiceCollection AddModbusProtocolConfig()
        {
            descriptors.AddProtocolConfigMapper<ModbusProtocolDescriptorConfigMapper>();
            descriptors.AddDataAddressMapper<ModbusProtocolAddressConfigMapper>();

            return descriptors;
        }
    }
}