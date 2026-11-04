namespace ThabeSoft.EdgeGateway.Tags;

public interface ITagWriter
{
    ValueTask WriteAsync<T>(TagDescriptor<T> tag, T value, CancellationToken cancellationToken = default)
        where T : notnull;
}
