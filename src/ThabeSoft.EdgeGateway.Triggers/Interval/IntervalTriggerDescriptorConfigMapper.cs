using ThabeSoft.EdgeGateway.Configuration;
using ThabeSoft.Guard;

namespace ThabeSoft.EdgeGateway.Triggers.Interval;


internal sealed class IntervalTriggerDescriptorConfigMapper : IConfigMapper<TriggerDescriptor>
{
    public bool CanMap(IConfigNode root)
    {
        if (!Enum.TryParse<TriggrType>(root.GetValue("Type"), true, out var enumValue))
            return false;

        return enumValue == TriggrType.Interval;
    }

    public TriggerDescriptor Map(IConfigNode root)
    {
        var timespan = root.GetValue("Interval").Required("缺少间隔时间")
                .ToTimeSpan();

        return TriggerBuilder.Interval(timespan);
    }
}