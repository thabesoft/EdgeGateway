namespace ThabeSoft.Triggers.Signal;


public sealed record class SignalTriggerDescriptor : TriggerDescriptor, ITriggerSignal
{
    public SemaphoreSlim Signal { get; } = new(0);
    public override TriggerType Type => TriggerType.Signal;


    void ITriggerSignal.OnSignal() => Signal.Release();
}