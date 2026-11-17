namespace ThabeSoft.EdgeGateway.Triggers;

public sealed class NotSupportedTriggerException(TriggerDescriptor trigger) :
    NotSupportedException($"不支持的触发器 [{trigger.GetType()}]: {trigger}")
{
    public TriggerDescriptor Trigger { get; } = trigger ?? throw new ArgumentNullException(nameof(trigger));
}