using ModbusSlaveSimulator;

#pragma warning disable IDE0130 // 命名空间与文件夹结构不匹配
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // 命名空间与文件夹结构不匹配


public static class DependencyInjectionExtensions
{
    extension(IServiceCollection descriptors)
    {
        public IServiceCollection AddModbusRtuSimulator(Action<ModbusSerialSimulatorOptions> optionsAction)
        {
            descriptors.Configure(optionsAction);
            descriptors.AddSingleton<ModbusRtuSimulator>();

            descriptors.AddSingleton<IModbusSerialSimulator>(x => x.GetRequiredService<ModbusRtuSimulator>());
            descriptors.AddSingleton<IModbusSimulator>(x => x.GetRequiredService<ModbusRtuSimulator>());

            return descriptors;
        }

        public IServiceCollection AddModbusIpSimulator(Action<ModbusIpSimulatorOptions> optionsAction)
        {
            descriptors.Configure(optionsAction);
            descriptors.AddSingleton<ModbusIpSimulator>();

            descriptors.AddSingleton<IModbusIpSimulator>(x => x.GetRequiredService<ModbusIpSimulator>());
            descriptors.AddSingleton<IModbusSimulator>(x => x.GetRequiredService<ModbusIpSimulator>());

            return descriptors;
        }
    }
}
