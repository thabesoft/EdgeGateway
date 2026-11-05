using Microsoft.Extensions.Logging;
using ThabeSoft.EdgeGateway.Lifecycles;
using ThabeSoft.EdgeGateway.Tags;
using ThabeSoft.EdgeGateway.Triggers;

namespace ThabeSoft.EdgeGateway.Polling;


internal sealed class Subscription<T> : LifecycleStateMachine, ISubscription<T>
    where T : notnull
{
    private readonly ILogger<Subscription<T>> _logger;
    private readonly ITrigger _trigger;
    private readonly OnTickHandler _onTickHandler;

    private Task? _loopTask;
    private CancellationTokenSource? _loopCts;


    public Subscription(ITrigger trigger, ITag<T> tag, OnTickHandler onTickHandler, ILogger<Subscription<T>> logger)
    {
        _logger = logger;
        _trigger = trigger;
        _onTickHandler = onTickHandler;

        Tag = tag;
    }


    public ITag<T> Tag { get; }
    ITag ISubscription.Tag => Tag;



    protected override ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        if (_loopTask is { IsCompleted: false })
            return ValueTask.CompletedTask;

        _loopCts = new CancellationTokenSource();
        _loopTask = TriggerTask(_loopCts.Token);
        return ValueTask.CompletedTask;
    }
    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        if (_loopCts is null || _loopTask is null)
            return;

        await _loopCts.CancelAsync();

        try { await _loopTask; }
        catch (OperationCanceledException) { }
        catch (Exception ex) { _logger.LogError(ex, "触发器任务停止失败"); }

        _loopCts.Dispose();
        _loopCts = null;
        _loopTask = null;
    }

    private async Task TriggerTask(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (!await _trigger.WaitForNextTickAsync(cancellationToken))
                return;

            await _onTickHandler.Invoke(cancellationToken);
        }
    }

    public delegate ValueTask OnTickHandler(CancellationToken cancellationToken = default);
}