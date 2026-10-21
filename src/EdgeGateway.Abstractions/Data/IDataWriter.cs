using FluentResults;

namespace EdgeGateway.Data;

public interface IDataWriter
{
    ValueTask<Result<T>> WriteAsync<T>(DataDescriptor<T> descriptor, T value, CancellationToken cancellationToken = default);
}