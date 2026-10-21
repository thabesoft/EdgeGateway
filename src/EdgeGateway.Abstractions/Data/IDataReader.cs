using FluentResults;

namespace EdgeGateway.Data;

public interface IDataReader
{
    ValueTask<Result<T>> ReadAsync<T>(DataDescriptor<T> descriptor, CancellationToken cancellationToken = default);
}
