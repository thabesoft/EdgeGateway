using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ThabeSoft.Protocols.Accessor;

namespace ThabeSoft.EdgeGateway.Readings;


internal sealed class ReadingSource<T>(
    DataAccessDescriptor<T> tagDescriptor,
    IProtocolAccessor protocolAccessor,
    ILogger<ReadingSource<T>>? logger = null
    ) where T : notnull
{
    private readonly ILogger<ReadingSource<T>> _logger = logger ?? NullLogger<ReadingSource<T>>.Instance;

    public Reading<T>? Latest { get; private set; }


    public async ValueTask<Reading<T>> ReadAsync(CancellationToken cancellationToken)
    {
        try
        {
            var value = await protocolAccessor.ReadAsync(tagDescriptor, cancellationToken);

            var reading = Reading.Good(value);
            Latest = reading;

            return reading;
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "订阅读取失败");
            return Reading.Bad(GetLatestValue());
        }
    }

    private T GetLatestValue() => Latest is null ? default! : Latest.Value.Value;
}