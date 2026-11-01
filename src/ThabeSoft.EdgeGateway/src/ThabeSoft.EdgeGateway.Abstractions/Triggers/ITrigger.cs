namespace ThabeSoft.EdgeGateway.Triggers;


public interface ITrigger
{
    IDisposable Subscribe(TriggerHandler handler);
}

public delegate ValueTask TriggerHandler(CancellationToken cancellationToken = default);