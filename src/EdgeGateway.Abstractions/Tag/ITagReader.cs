namespace EdgeGateway.Tag;

public interface ITagReader
{
    ValueTask<T> ReadAsync<T>(ITag<T> tag, CancellationToken cancellationToken = default)
        where T : notnull;
}