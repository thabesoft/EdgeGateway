namespace ThabeSoft.EdgeGateway.Tags;

public interface ITagReader
{
    ValueTask<T> ReadAsync<T>(ITagDescriptor<T> tag, CancellationToken cancellationToken = default)
        where T : notnull;
}