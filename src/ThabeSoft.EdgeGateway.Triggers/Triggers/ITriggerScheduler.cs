namespace ThabeSoft.EdgeGateway.Triggers;

public interface ITriggerScheduler
{
    IDisposable Register(ITriggerDescriptor triggerDescriptor, Func<CancellationToken, ValueTask> onTickAsync);
}
