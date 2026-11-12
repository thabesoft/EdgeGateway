using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace ThabeSoft.EdgeGateway.Events;


internal class EventPublisher(IServiceProvider services, ILogger<EventPublisher> logger) : IEventPublisher, IAsyncDisposable
{
    private readonly CancellationTokenSource _inFlightCts = new();
    private readonly ConcurrentBag<Task> _inFlight = [];


    public void Publish<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var handlers = services.GetServices<IEventHandler<TEvent>>().ToArray();
        if (handlers.Length == 0) return;

        var task = OnPublishAsync(@event, handlers);
        _inFlight.Add(task);
        _ = task.ContinueWith(_ => _inFlight.TryTake(out _), TaskScheduler.Default);
    }

    public async ValueTask DisposeAsync()
    {
        await _inFlightCts.CancelAsync();

        var tasks = _inFlight.ToArray();
        try { await Task.WhenAll(tasks); }
        catch (OperationCanceledException) { }
        catch (Exception ex) { logger.LogError(ex, "等待在飞事件任务时出错"); }

        _inFlightCts.Dispose();
    }

    private async Task OnPublishAsync<TEvent>(TEvent @event, IEventHandler<TEvent>[] eventHandlers)
        where TEvent : IEvent
    {
        foreach (var handler in eventHandlers)
        {
            try
            {
                await handler.HandleAsync(@event, _inFlightCts.Token);
            }
            catch (OperationCanceledException) when (_inFlightCts.IsCancellationRequested)
            {
                logger.LogTrace("事件({Event}) 取消发布", typeof(TEvent));
                return;
            }
            catch (OperationCanceledException)
            {
                logger.LogTrace("事件({Event}) 处理器({Handler}) 取消执行", typeof(TEvent), handler.GetType());
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "事件({Event}) 处理器({Handler}) 执行失败", typeof(TEvent), handler.GetType());
            }
        }
    }
}
