using EdgeGateway.Devices;
using EdgeGateway.Options;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace EdgeGateway.Ports;


public interface IPortManager
{
    IReadOnlyDictionary<PortAddress, IPortHandle> Ports { get; }

    Result<IPortHandle> Add(IPortOptions options);
    ValueTask<Result> RemovAsync(PortAddress address, CancellationToken cancellationToken = default);
}

public sealed class PortManager(IPortFactoryManager portFactoryManager, IDeviceManager deviceManager, ILogger<PortManager> logger) : IPortManager
{
    private readonly Dictionary<PortAddress, IPortHandle> _portHandles = [];

    public IReadOnlyDictionary<PortAddress, IPortHandle> Ports => _portHandles;

    public Result<IPortHandle> Add(IPortOptions config)
    {
        var createErrors = new List<IError>();

        foreach (var factory in portFactoryManager.Factories.ToArray())
        {
            if (!factory.CanHandle(config)) continue;

            var createResult = factory.Create(config);
            if (createResult.IsFailed)
            {
                createErrors.Add(new Error($"{factory.GetType()}").CausedBy(createResult.Errors));
                continue;
            }

            var port = createResult.Value;
            _portHandles[port.Address] = new PortHandle(this, deviceManager, port, logger);

            return Result.Ok(_portHandles[port.Address]);
        }

        return Result.Fail("没有工厂能处理该配置)").WithErrors(createErrors);
    }

    public async ValueTask<Result> RemovAsync(PortAddress address, CancellationToken cancellationToken = default)
    {
        if (!_portHandles.TryGetValue(address, out var portHandle))
            return Result.Ok();

        Result closeResult = await portHandle.CloseAsync(cancellationToken);
        if (closeResult.IsFailed)
            return Result.Fail("端口无法移除").WithErrors(closeResult.Errors);

        _portHandles.Remove(portHandle.Address);
        return Result.Ok();
    }
}