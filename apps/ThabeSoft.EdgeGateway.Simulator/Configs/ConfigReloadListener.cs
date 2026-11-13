using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ThabeSoft.EdgeGateway.Simulator.Configs;


internal class ConfigReloadListener(ConfigScriptRunner configScriptRunner, ILogger<ConfigReloadListener> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await configScriptRunner.RunAsync(stoppingToken);

                // 等待重新加载命令
                while (!stoppingToken.IsCancellationRequested)
                {
                    var input = await Console.In.ReadLineAsync(stoppingToken);
                    if (input?.Trim().Equals("R", StringComparison.OrdinalIgnoreCase) == true) break;
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("配置脚本已取消执行");
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "配置脚本执行失败");
            }
        }
    }
}
