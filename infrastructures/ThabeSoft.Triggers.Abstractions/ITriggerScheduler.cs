namespace ThabeSoft.Triggers;

public interface ITriggerScheduler
{
    IDisposable Register(TriggerDescriptor triggerDescriptor, Func<CancellationToken, ValueTask> onTickAsync);
}
