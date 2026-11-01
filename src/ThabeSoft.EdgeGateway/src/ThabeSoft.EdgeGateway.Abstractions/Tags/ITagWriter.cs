namespace ThabeSoft.EdgeGateway.Tags;

public interface ITagWriter
{
    ValueTask WriteAsync<T>(ITag<T> tag, T value, CancellationToken cancellationToken = default)
        where T : notnull;
}
