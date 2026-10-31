using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Simulator;

namespace ThabeSoft.EdgeGateway.Demo.Services;


internal sealed class ModbusSimulatorHostedService(IModbusSimulator modbusSimulator, ILogger<GatewayHostedService> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await modbusSimulator.StartAsync(cancellationToken);
        logger.LogInformation("Modbus从站模拟器已启动");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await modbusSimulator.StopAsync(cancellationToken);
        logger.LogInformation("Modbus从站模拟器已停止");
    }
}