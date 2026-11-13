namespace ThabeSoft.EdgeGateway.Triggers.Signal;


public sealed class SignalTriggerDescriptor : ITriggerDescriptor, ISignal
{
    public SemaphoreSlim Signal { get; } = new(0);

    public bool Equals(ITriggerDescriptor other) =>
        other is SignalTriggerDescriptor;
    public override string ToString() => $"Signal";


    void ISignal.OnSignal() => Signal.Release();
}

public interface ISignal
{
    void OnSignal();
}