namespace ThabeSoft.EdgeGateway.Triggers.Signal;


public sealed record class SignalTriggerDescriptor : TriggerDescriptor, ITriggerSignal
{
    public SemaphoreSlim Signal { get; } = new(0);
    public override TriggrType Type => TriggrType.Signal;


    void ITriggerSignal.OnSignal() => Signal.Release();
}