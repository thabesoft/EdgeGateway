using FluentResults;

namespace EdgeGateway;


public interface IGateway : IAsyncDisposable
{
    /// <summary>
    /// 指示网关当前是否正在运行。
    /// </summary>
    bool IsRunning { get; }


    ValueTask<Result> StartAsync(CancellationToken cancellationToken = default);
    ValueTask<Result> StopAsync(CancellationToken cancellationToken = default);
}