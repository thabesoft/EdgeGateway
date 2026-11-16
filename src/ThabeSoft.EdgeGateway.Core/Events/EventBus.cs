using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Events;


internal sealed class EventBus(ILogger<EventBus> logger) :
    IEventPublisher, IEventSubscriber, IAsyncDisposable
{
    private readonly ConcurrentDictionary<Guid, Func<IEvent, CancellationToken, Task>> _handlers = [];
    private readonly CancellationTokenSource _publishTaskCts = new();
    private readonly ConcurrentDictionary<Guid, Task> _publishTasks = [];



    public IDisposable Subscribe<TEvent>(Func<TEvent, CancellationToken, ValueTask> handler) where TEvent : IEvent
    {
        var id = Guid.NewGuid();
        _handlers[id] = Warpper;

        return Disposer.From(() => _handlers.TryRemove(id, out var handler));


        // 事件包装
        async Task Warpper(IEvent @event, CancellationToken cancellationToken)
        {
            if (@event is not TEvent tEvent)
                return;

            try
            {
                await handler.Invoke(tEvent, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogTrace("事件({Type})取消处理", typeof(TEvent));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "事件({Type})处理失败", typeof(TEvent));
            }
        }
    }

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : IEvent
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _publishTaskCts.Token);
        var tasks = _handlers.Values.Select(x => x.Invoke(@event, cts.Token)).ToArray();

        var publishId = Guid.NewGuid();
        var publishTask = Task.WhenAll(tasks);
        _publishTasks[publishId] = publishTask;

        try
        {
            await publishTask;
        }
        finally
        {
            _publishTasks.TryRemove(publishId, out _);
        }
    }


    public async ValueTask DisposeAsync()
    {
        await _publishTaskCts.CancelAsync();

        var pending = _publishTasks.Values.ToArray();
        if (pending.Length > 0)
        {
            try { await Task.WhenAll(pending); }
            catch (OperationCanceledException) { }
            catch (Exception ex) { logger.LogError(ex, "事件总线释放失败"); }
        }

        _publishTaskCts.Dispose();
    }
}