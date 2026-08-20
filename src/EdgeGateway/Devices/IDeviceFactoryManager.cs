using FluentResults;

namespace EdgeGateway.Devices;

public interface IDeviceFactoryManager
{
#if NETSTANDARD2_0
    IReadOnlyCollection<IDeviceFactory> Factories { get; }
#else
    IReadOnlySet<IDeviceFactory> Factories { get; }
#endif

    Result Register(IDeviceFactory factory);
    void Remove(IDeviceFactory factory);
}


public sealed class DeviceFactoryManager : IDeviceFactoryManager
{
    public static DeviceFactoryManager Default { get; } = new();


    private readonly HashSet<IDeviceFactory> _deviceFactories = [];


#if NETSTANDARD2_0
    IReadOnlyCollection<IDeviceFactory> IDeviceFactoryManager.Factories => _deviceFactories;
#else
     IReadOnlySet<IDeviceFactory> IDeviceFactoryManager.Factories => _deviceFactories;
#endif

    public Result Register(IDeviceFactory factory)
    {
        if (_deviceFactories.Add(factory)) return Result.Ok();
        return new Error("该工厂实例已注册");
    }
    public void Remove(IDeviceFactory factory)
    {
        _deviceFactories.Remove(factory);
    }
}