using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using ThabeSoft.EdgeGateway.Utilities;

namespace ThabeSoft.EdgeGateway.Triggers;

internal sealed class TriggerManager(ITriggerResolver triggerFactorySelector, ILoggerFactory loggerFactory)
{
    private readonly ConcurrentDictionary<TriggerDescriptor, TriggerScheduler> _entries = [];

    public async ValueTask<TriggerScheduler> GetSchedulerAsync(TriggerDescriptor triggerDescriptor, CancellationToken cancellationToken)
    {
        var scheduler = _entries.GetOrAdd(triggerDescriptor, x =>
        {
            var trigger = triggerFactorySelector.Resolve(x);
            return new TriggerScheduler(trigger, loggerFactory.CreateLogger<TriggerScheduler>());
        });

        await scheduler.StartAsync(cancellationToken);
        return scheduler;
    }

}