using ThabeSoft.EdgeGateway.Tags;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Triggers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.EdgeGateway.Lifecycles;

namespace ThabeSoft.EdgeGateway.Polling;


internal sealed class PollingScheduler(ITagReader tagReader, ILogger<PollingScheduler> logger) : ResilientLifecycleBase(logger), IPollingScheduler
{
    private readonly ConcurrentDictionary<TagName, Context> _context = [];


    public PollingScheduler(ITagReader tagReader) : this(tagReader, NullLogger<PollingScheduler>.Instance) { }

    public IReadOnlyDictionary<TagName, ITagMonitor> TagMonitors => _context.ToDictionary(k => k.Key, v => v.Value.Monitor);


    public ITagMonitor<T> Register<T>(ITagDescriptor<T> descriptor, ITrigger trigger) where T : notnull
    {
        var monitor = new TagMonitor<T>(descriptor);
        var disposer = trigger.Subscribe(async ct =>
        {
            try
            {
                logger.LogDebug("触发器回调, 开始读取标签值");

                var value = await tagReader.ReadAsync(descriptor, ct);
                monitor.SetValue(value);

                logger.LogDebug("标签值已更新: {value}", value);
            }
            catch (Exception ex)
            {
                monitor.SetError(ex);
                logger.LogDebug(ex, "标签读取失败");
            }
        });

        _context[descriptor.Name] = new(disposer)
        {
            Monitor = monitor,
            Trigger = trigger
        };
        return monitor;
    }


    public void Unregister(TagName tagName)
    {
        if (!_context.TryRemove(tagName, out var context))
            return;

        context.Dispose();
    }


    protected override async ValueTask OnStartAsync(CancellationToken cancellationToken)
    {
        foreach (var context in _context)
        {
            if (context.Value.Trigger is not ILifecycle lifecycle)
                continue;

            try
            {
                await lifecycle.StartAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "启动触发器 {TagName} 失败", context.Key);
            }
        }
    }

    protected override async ValueTask OnStopAsync(CancellationToken cancellationToken)
    {
        var errors = new List<Exception>();

        foreach (var context in _context)
        {
            if (context.Value.Trigger is not ILifecycle lifecycle)
                continue;

            try
            {
                await lifecycle.StopAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
                logger.LogError(ex, "停止 Tag [{TagName}] 触发器失败", context.Key);
            }
        }

        if (errors.Count == 0) return;
        throw new AggregateException("部分触发器停止失败，调度器状态可能未完全清理", errors);
    }

    protected override ValueTask OnDisposeAsync()
    {
        var contexts = _context.Values.ToArray();
        _context.Clear();

        foreach (var i in contexts)
        {
            i.Dispose();
        }

        return ValueTask.CompletedTask;
    }

    private class Context(IDisposable disposer) : IDisposable
    {
        public required ITagMonitor Monitor { get; init; }
        public required ITrigger Trigger { get; init; }

        public void Dispose() => disposer?.Dispose();
    }
}
