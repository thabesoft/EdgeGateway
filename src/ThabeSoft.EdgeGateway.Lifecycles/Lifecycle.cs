using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway;


public static class Lifecycle
{
    /// <summary>
    /// 使用委托创建生命周期
    /// </summary>
    public static IDisposableLifecycle Background(Func<CancellationToken, Task> backgroundAction, ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(backgroundAction);
        return new BackgroundLifecycle(backgroundAction, logger);
    }
}