using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using ThabeSoft.Primitives;

namespace ThabeSoft.PubSub;


internal sealed class EventBus(ILogger<EventBus> logger) :
    IEventPublisher, IEventSubscriber, IAsyncDisposable
{
    private readonly ConcurrentDictionary<Guid, Func<IMessage, CancellationToken, Task>> _handlers = [];
    private readonly CancellationTokenSource _publishTaskCts = new();
    private readonly ConcurrentDictionary<Guid, Task> _publishTasks = [];


    public IDisposable Subscribe<TMessage>(Func<TMessage, CancellationToken, ValueTask> handler) where TMessage : IMessage
    {
        var id = Guid.NewGuid();
        _handlers[id] = Warpper;

        return Disposer.From(() => _handlers.TryRemove(id, out var handler));


        // 事件包装
        async Task Warpper(IMessage message, CancellationToken cancellationToken)
        {
            if (message is not TMessage msg)
                return;

            try
            {
                logger.LogTrace("事件({Type})回调", typeof(TMessage));
                await handler.Invoke(msg, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                logger.LogTrace("事件({Type})取消处理", typeof(TMessage));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "事件({Type})处理失败", typeof(TMessage));
            }
        }
    }

    public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : IMessage
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _publishTaskCts.Token);
        var tasks = _handlers.Values.Select(x => x.Invoke(message, cts.Token)).ToArray();

        var publishId = Guid.NewGuid();
        var publishTask = Task.WhenAll(tasks);
        _publishTasks[publishId] = publishTask;

        try
        {
            logger.LogTrace("发布事件({Type}), Content({Content})", typeof(TMessage), message);
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