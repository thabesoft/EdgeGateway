using EdgeGateway.Options.Monitor;
using EdgeGateway.Ports;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace EdgeGateway;


/// <summary>
/// 根据配置自动更新的网关
/// </summary>
public sealed class Gateway : ILifetime
{
    private readonly IDisposable _optionsSubcribeDisposer;
    private readonly IPortManager _portManager;
    private readonly ILogger<Gateway> _logger;


    public bool IsRunning { get; private set; }


    public Gateway(
        IGatewayOptionsMonitor monitor,
        IPortManager portManager,
        ILogger<Gateway> logger)
    {
        _portManager = portManager;
        _logger = logger;


        _optionsSubcribeDisposer = monitor.PortAdded.Subscribe(x => portManager.Add(x));
        monitor.PortRemoved.Subscribe(async x => await portManager.RemoveAsync(x.Address), CancellationToken.None);

        //monitor.Port.Removed((x, ct) => portManager.RemovAsync(x.Address, ct));
    }

    public async ValueTask<Result> StartAsync(CancellationToken cancellationToken = default)
    {
        var openErrors = new List<IError>();

        foreach (var i in _portManager.Ports.Values.ToArray())
        {
            var openResult = await i.OpenAsync(cancellationToken);
            if (openResult.IsSuccess) continue;

            openErrors.Add(new Error($"{i.Address}").CausedBy(openResult.Errors));
        }

        IsRunning = true;

        if (openErrors.Count <= 0) return Result.Ok();
        return Result.Fail("开启失败").WithErrors(openErrors);
    }
    public async ValueTask<Result> StopAsync(CancellationToken cancellationToken = default)
    {
        var closeErrors = new List<IError>();

        foreach (var i in _portManager.Ports.Values.ToArray())
        {
            var closeResult = await i.CloseAsync(cancellationToken);
            if (closeResult.IsSuccess) continue;

            closeErrors.Add(new Error($"{i.Address}").CausedBy(closeResult.Errors));
        }

        IsRunning = false;

        if (closeErrors.Count <= 0) return Result.Ok();
        return Result.Fail("停止失败").WithErrors(closeErrors);
    }
    public async ValueTask DisposeAsync()
    {
        try
        {
            _optionsSubcribeDisposer.Dispose();
            await StopAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "释放失败");
        }
    }
}