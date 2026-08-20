using EdgeGateway.Devices;
using EdgeGateway.Logger;
using EdgeGateway.Ports;
using EdgeGateway.Tags;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace EdgeGateway;


public sealed class Gateway(IPortManager portManager, ILogger<Gateway> logger) : IGateway
{
    public bool IsRunning { get; private set; }


    public async ValueTask<Result> StartAsync(CancellationToken cancellationToken = default)
    {
        var openErrors = new List<IError>();

        foreach (var i in portManager.Ports.Values.ToArray())
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

        foreach (var i in portManager.Ports.Values.ToArray())
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
            await StopAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "释放失败");
        }
    }



    public static GatewayContext Create(ILoggerFactory loggerFactory)
    {
        var deviceFactoryManager = new DeviceFactoryManager();
        var deviceManager = new DeviceManager(deviceFactoryManager, loggerFactory.CreateLogger<DeviceManager>());

        var portFactoryManager = new PortFactoryManager();
        var portManager = new PortManager(portFactoryManager, deviceManager, loggerFactory.CreateLogger<PortManager>());

        var tagReader = new TagReader(portManager, deviceManager);
        var tagWriter = new TagWriter(portManager, deviceManager);

        var gateway = new Gateway(portManager, loggerFactory.CreateLogger<Gateway>());

        return new(gateway)
        {
            Port = portManager,
            PortFactory = portFactoryManager,

            Device = deviceManager,
            DeviceFactory = deviceFactoryManager,

            TagReader = tagReader,
            TagWriter = tagWriter
        };
    }

    public static GatewayContext Create(LogLevel minLevel = LogLevel.Information)
    {
        return Create(ConsoleLoggerFactory.FromLevel(minLevel));
    }
}
