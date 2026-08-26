using FluentResults;

namespace EdgeGateway.Ports;


public interface IPortFactory
{
    /// <summary>
    /// 判断配置是否可以被该工厂处理
    /// </summary>
    bool CanHandle(IPortOptions options);

    Result<IPort> Create(IPortOptions options);
}