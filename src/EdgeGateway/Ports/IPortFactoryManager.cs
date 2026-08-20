using FluentResults;

namespace EdgeGateway.Ports;


public interface IPortFactoryManager
{
#if NETSTANDARD2_0
    IReadOnlyCollection<IPortFactory> Factories { get; }
#else
    IReadOnlySet<IPortFactory> Factories { get; }
#endif

    Result Register(IPortFactory factory);
    void Remove(IPortFactory factory);
}


public sealed class PortFactoryManager : IPortFactoryManager
{
    private readonly HashSet<IPortFactory> _factories = [];

#if NETSTANDARD2_0
    IReadOnlyCollection<IPortFactory> IPortFactoryManager.Factories => _factories;
#else
     IReadOnlySet<IPortFactory> IPortFactoryManager.Factories => _factories;
#endif

    public Result Register(IPortFactory factory)
    {
        if (_factories.Add(factory)) return Result.Ok();

        return new Error("该工厂实例已注册");
    }
    public void Remove(IPortFactory factory)
    {
        _factories.Remove(factory);
    }
}