using Microsoft.Extensions.Hosting;
using ThabeSoft.Simulator;
using ThabeSoft.Simulator.Ethernet;
using ThabeSoft.Simulator.Serial;


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
            descriptors.AddHostedService<ModbusSimulatorHostedService>();

            return descriptors;
        }

        public IServiceCollection AddModbusIpSimulator(Action<ModbusIpSimulatorOptions> optionsAction)
        {
            descriptors.Configure(optionsAction);
            descriptors.AddSingleton<ModbusEthernetSimulator>();

            descriptors.AddSingleton<IModbusIpSimulator>(x => x.GetRequiredService<ModbusEthernetSimulator>());
            descriptors.AddSingleton<IModbusSimulator>(x => x.GetRequiredService<ModbusEthernetSimulator>());

            return descriptors;
        }
    }
}


internal sealed class ModbusSimulatorHostedService(IModbusSimulator simulator) : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken) =>
        simulator.StartAsync(cancellationToken);

    public Task StopAsync(CancellationToken cancellationToken) =>
        simulator.StopAsync(cancellationToken);
}