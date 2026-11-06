namespace ThabeSoft.EdgeGateway.Triggers;


internal sealed class SignalTriggerDescriptor : ITriggerDescriptor, ISignal
{
    public SemaphoreSlim Signal { get; } = new(0);
    void ISignal.OnSignal() => Signal.Release();
}

public interface ISignal
{
    void OnSignal();
}