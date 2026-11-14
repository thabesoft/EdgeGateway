using ThabeSoft.EdgeGateway.Protocols;
using ThabeSoft.EdgeGateway.Tags;

namespace ThabeSoft.EdgeGateway.Subscription;


internal sealed class SubscriptionReader<T>(
    TagDescriptor<T> tagDescriptor,
    IProtocolAccessor protocolAccessor,
    Func<Exception, bool> recovery
    ) where T : notnull
{
    public Reading<T>? Latest { get; private set; }


    public async ValueTask<Reading<T>> ReadAsync(CancellationToken ct)
    {
        try
        {
            var value = await protocolAccessor.ReadAsync(tagDescriptor, ct);

            var reading = Reading.Good(value);
            Latest = reading;

            return reading;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            //    if(recovery.Invoke(ex))
            return Reading.Bad(GetLatestValue());
        }
    }

    private T GetLatestValue() => Latest is null ? default! : Latest.Value.Value;
}