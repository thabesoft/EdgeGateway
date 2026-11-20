using ThabeSoft.Configuration.Models;
using ThabeSoft.EdgeGateway.Subscription;

namespace ThabeSoft.Configuration.Diff;


internal sealed class ConfigDiffCalculator
{
    public void Diff(GatewayConfig old, GatewayConfig @new)
    {
        var a = old.Ports.SelectMany(x => x.Devices.SelectMany(y => y.Subscriptions));





        var result = Diff(old.Ports, @new.Ports);
    }


    private static DiffResult<T> Diff<T>(IReadOnlyList<T> olds, IReadOnlyList<T> news)
        where T : IEquatable<T>
    {
        var unchanged = new HashSet<T>(olds.Intersect(news));
        var addd = new HashSet<T>();
        var removed = new HashSet<T>();

        // 新元素中没有老元素, 删除老元素
        foreach (var oldItem in olds)
        {
            if (!unchanged.Contains(oldItem)) removed.Add(oldItem);
        }

        // 老元素中没有新元素, 新增新元素
        foreach (var newItem in news)
        {
            if (!unchanged.Contains(newItem)) addd.Add(newItem);
        }

        return new()
        {
            Added = [.. addd],
            Unchanged = [.. unchanged],
            Removed = [.. removed]
        };
    }

    private class DiffResult<T>
    {
        public required IReadOnlyList<T> Added { get; init; }
        public required IReadOnlyList<T> Removed { get; init; }
        public required IReadOnlyList<T> Unchanged { get; init; }
    }
}


interface IDiffExecutor
{
    ValueTask AddedSubscription(SubscriptionConfig config);
    ValueTask RemoveSubscription(SubscriptionId id);
    ValueTask UpdateSubscription(SubscriptionConfig config);
}