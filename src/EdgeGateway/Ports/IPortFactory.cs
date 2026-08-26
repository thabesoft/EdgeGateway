using EdgeGateway.Options;
using FluentResults;

namespace EdgeGateway.Ports;


public interface IPortFactory
{
    /// <summary>
    /// 判断配置是否可以被该工厂处理
    /// </summary>
    bool CanHandle(IPortOptions config);

    Result<IPort> Create(IPortOptions config);
}