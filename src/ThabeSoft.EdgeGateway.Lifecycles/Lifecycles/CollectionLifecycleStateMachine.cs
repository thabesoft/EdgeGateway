using Microsoft.Extensions.Logging;

namespace ThabeSoft.EdgeGateway.Lifecycles;

public sealed class CollectionLifecycleStateMachine : LifecycleStateMachineA
{
    private readonly ILogger _logger;


    internal CollectionLifecycleStateMachine(ILogger logger) : base(logger) => _logger = logger;


    public required TimeSpan ItemStartTimeout { get; init; }
    public required TimeSpan ItemStopTimeout { get; init; }
    public required TimeSpan ItemDisposeTimeout { get; init; }


    public ValueTask StartAllAsync(ILifecycle[] lifecycles, CancellationToken cancellationToken = default)
    {
        return StartAsync(
            state: (Items: lifecycles, Logger: _logger, Timeout: ItemStartTimeout),
            onStart: static async (state, ct) =>
            {
                List<Exception> errors = [];

                foreach (var lifecycle in state.Items)
                {
                    try
                    {
                        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                        cts.CancelAfter(state.Timeout);

                        await lifecycle.StartAsync(ct);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex);
                        state.Logger.LogError(ex, "{Type} 启动失败", lifecycle.GetType());
                    }
                }

                if (errors.Count == 0) return;
                throw new AggregateException($"部分启动失败, 共 {errors.Count} 个", errors);

            },
            cancellationToken: cancellationToken);
    }
    public ValueTask StopAllAsync(ILifecycle[] lifecycles, CancellationToken cancellationToken = default)
    {
        return StopAsync(
            state: (Items: lifecycles, Logger: _logger, Timeout: ItemStopTimeout),
            onStop: static async (state, ct) =>
            {
                List<Exception> errors = [];

                foreach (var lifecycle in state.Items)
                {
                    try
                    {
                        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                        cts.CancelAfter(state.Timeout);

                        await lifecycle.StopAsync(ct);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex);
                        state.Logger.LogError(ex, "{Type} 停止失败", lifecycle.GetType());
                    }
                }

                if (errors.Count == 0) return;
                throw new AggregateException($"部分停止失败, 共 {errors.Count} 个", errors);

            },
            cancellationToken: cancellationToken);
    }
    public ValueTask DisposeAllAsync(ILifecycle[] lifecycles)
    {
        return DisposeAsync(
            state: (Items: lifecycles, Logger: _logger, Timeout: ItemStopTimeout),
            onDispose: static async state =>
            {
                foreach (var lifecycle in state.Items)
                {
                    try
                    {
                        //TODO: 没有写元素的释放超时

                        if (lifecycle is IAsyncDisposable asyncDisposable)
                            await asyncDisposable.DisposeAsync();
                        else if (lifecycle is IDisposable disposable)
                            disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        state.Logger.LogError(ex, "{Type} 释放失败", lifecycle.GetType());
                    }
                }
            });
    }
}