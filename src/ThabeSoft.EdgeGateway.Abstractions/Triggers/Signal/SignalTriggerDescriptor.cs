namespace ThabeSoft.EdgeGateway.Triggers.Signal;


public sealed record class SignalTriggerDescriptor : TriggerDescriptor, ISignal
{
    public SemaphoreSlim Signal { get; } = new(0);
    void ISignal.OnSignal() => Signal.Release();
}

public interface ISignal
{
    void OnSignal();
}