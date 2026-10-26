namespace EdgeGateway.Tag;

public interface ITagWriter
{
    ValueTask WriteAsync<T>(ITag<T> tag, T value, CancellationToken cancellationToken = default);
}
