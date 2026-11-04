namespace ThabeSoft.EdgeGateway.Tags;

public interface ITagReader
{
    ValueTask<T> ReadAsync<T>(TagDescriptor<T> tag, CancellationToken cancellationToken = default)
        where T : notnull;
}