using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.EdgeGateway.Configuration.Nodes;
using ThabeSoft.Primitives.Guard;

namespace ThabeSoft.EdgeGateway.Triggers.Interval;


internal sealed class IntervalTriggerDescriptorConfigMapper : IConfigMapper<TriggerDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        if (!Enum.TryParse<TriggerType>(root.GetStringByPath("Type"), true, out var enumValue))
            return false;

        return enumValue == TriggerType.Interval;
    }

    public TriggerDescriptor Map(IConfigNode root)
    {
        var timespan = root.GetStringByPath("Interval").Required("缺少间隔时间")
                .ToTimeSpan();

        return TriggerBuilder.Interval(timespan);
    }
}